/**
 * Advanced Interactions
 * Phase 5: Infinite Scroll, Search Autocomplete, Image Zoom, Wishlist
 */

(function() {
    'use strict';

    // ===== INFINITE SCROLL =====
    class InfiniteScroll {
        constructor(options = {}) {
            this.container = options.container || document.querySelector('[data-infinite-scroll]');
            this.loadMoreUrl = options.loadMoreUrl || this.container?.dataset.loadMoreUrl;
            this.page = options.startPage || 1;
            this.loading = false;
            this.hasMore = true;
            this.threshold = options.threshold || 200;
            
            if (this.container) {
                this.init();
            }
        }

        init() {
            // Create loading indicator
            this.loadingIndicator = document.createElement('div');
            this.loadingIndicator.className = 'infinite-scroll-loading';
            this.loadingIndicator.innerHTML = `
                <div class="spinner"></div>
                <p class="text-sm text-text-muted mt-4">Đang tải thêm...</p>
            `;
            this.loadingIndicator.style.display = 'none';
            this.container.parentNode.insertBefore(this.loadingIndicator, this.container.nextSibling);

            // Setup intersection observer
            this.setupObserver();

            console.log('✅ Infinite Scroll initialized');
        }

        setupObserver() {
            const sentinel = document.createElement('div');
            sentinel.className = 'infinite-scroll-sentinel';
            sentinel.style.height = '1px';
            this.container.parentNode.insertBefore(sentinel, this.loadingIndicator);

            const observer = new IntersectionObserver((entries) => {
                entries.forEach(entry => {
                    if (entry.isIntersecting && !this.loading && this.hasMore) {
                        this.loadMore();
                    }
                });
            }, {
                rootMargin: `${this.threshold}px`
            });

            observer.observe(sentinel);
        }

        async loadMore() {
            if (this.loading || !this.hasMore) return;

            this.loading = true;
            this.loadingIndicator.style.display = 'block';

            try {
                this.page++;
                const url = `${this.loadMoreUrl}?page=${this.page}`;
                const response = await fetch(url);
                
                if (!response.ok) {
                    throw new Error('Failed to load more items');
                }

                const html = await response.text();
                
                if (html.trim() === '' || html.includes('no-more-items')) {
                    this.hasMore = false;
                    this.loadingIndicator.innerHTML = '<p class="text-sm text-text-muted">Đã hiển thị tất cả sản phẩm</p>';
                    return;
                }

                // Append new items
                const tempDiv = document.createElement('div');
                tempDiv.innerHTML = html;
                const newItems = tempDiv.querySelectorAll('[data-item]');
                
                newItems.forEach(item => {
                    this.container.appendChild(item);
                    item.classList.add('fade-in');
                });

                // Announce to screen reader
                if (window.announceToScreenReader) {
                    window.announceToScreenReader(`Đã tải thêm ${newItems.length} sản phẩm`, 'polite');
                }

            } catch (error) {
                console.error('Infinite scroll error:', error);
                this.loadingIndicator.innerHTML = `
                    <p class="text-sm text-primary">Không thể tải thêm. 
                    <button onclick="window.infiniteScroll.loadMore()" class="text-primary underline">Thử lại</button></p>
                `;
            } finally {
                this.loading = false;
                setTimeout(() => {
                    if (this.hasMore) {
                        this.loadingIndicator.style.display = 'none';
                    }
                }, 500);
            }
        }
    }

    // ===== SEARCH AUTOCOMPLETE =====
    class SearchAutocomplete {
        constructor(options = {}) {
            this.input = options.input || document.querySelector('[data-search-autocomplete]');
            this.searchUrl = options.searchUrl || '/api/search';
            this.minChars = options.minChars || 2;
            this.debounceTime = options.debounceTime || 300;
            this.maxResults = options.maxResults || 8;
            
            if (this.input) {
                this.init();
            }
        }

        init() {
            // Create results container
            this.resultsContainer = document.createElement('div');
            this.resultsContainer.className = 'search-autocomplete-results';
            this.resultsContainer.setAttribute('role', 'listbox');
            this.resultsContainer.style.display = 'none';
            this.input.parentNode.style.position = 'relative';
            this.input.parentNode.appendChild(this.resultsContainer);

            // Setup event listeners
            this.input.addEventListener('input', window.debounce((e) => {
                this.handleInput(e.target.value);
            }, this.debounceTime));

            this.input.addEventListener('keydown', (e) => {
                this.handleKeydown(e);
            });

            this.input.addEventListener('focus', () => {
                if (this.results && this.results.length > 0) {
                    this.showResults();
                }
            });

            // Close on outside click
            document.addEventListener('click', (e) => {
                if (!this.input.contains(e.target) && !this.resultsContainer.contains(e.target)) {
                    this.hideResults();
                }
            });

            // ARIA attributes
            this.input.setAttribute('role', 'combobox');
            this.input.setAttribute('aria-autocomplete', 'list');
            this.input.setAttribute('aria-expanded', 'false');
            this.input.setAttribute('aria-controls', 'search-results');
            this.resultsContainer.id = 'search-results';

            console.log('✅ Search Autocomplete initialized');
        }

        async handleInput(query) {
            if (query.length < this.minChars) {
                this.hideResults();
                return;
            }

            try {
                const response = await fetch(`${this.searchUrl}?q=${encodeURIComponent(query)}&limit=${this.maxResults}`);
                const data = await response.json();
                
                this.results = data.results || data;
                this.renderResults();
                
            } catch (error) {
                console.error('Search autocomplete error:', error);
            }
        }

        renderResults() {
            if (!this.results || this.results.length === 0) {
                this.resultsContainer.innerHTML = `
                    <div class="search-autocomplete-item no-results">
                        <i class="fas fa-search text-text-muted"></i>
                        <span>Không tìm thấy kết quả</span>
                    </div>
                `;
                this.showResults();
                return;
            }

            this.resultsContainer.innerHTML = this.results.map((result, index) => `
                <a href="${result.url}" 
                   class="search-autocomplete-item" 
                   role="option"
                   data-index="${index}">
                    ${result.image ? `<img src="${result.image}" alt="${result.title}" class="search-result-image">` : ''}
                    <div class="search-result-content">
                        <div class="search-result-title">${this.highlightMatch(result.title, this.input.value)}</div>
                        ${result.category ? `<div class="search-result-category">${result.category}</div>` : ''}
                        ${result.price ? `<div class="search-result-price">${result.price}</div>` : ''}
                    </div>
                </a>
            `).join('');

            this.showResults();
            this.selectedIndex = -1;
        }

        highlightMatch(text, query) {
            const regex = new RegExp(`(${query})`, 'gi');
            return text.replace(regex, '<mark>$1</mark>');
        }

        showResults() {
            this.resultsContainer.style.display = 'block';
            this.input.setAttribute('aria-expanded', 'true');
        }

        hideResults() {
            this.resultsContainer.style.display = 'none';
            this.input.setAttribute('aria-expanded', 'false');
        }

        handleKeydown(e) {
            const items = this.resultsContainer.querySelectorAll('.search-autocomplete-item');
            
            if (items.length === 0) return;

            switch(e.key) {
                case 'ArrowDown':
                    e.preventDefault();
                    this.selectedIndex = Math.min(this.selectedIndex + 1, items.length - 1);
                    this.updateSelection(items);
                    break;
                case 'ArrowUp':
                    e.preventDefault();
                    this.selectedIndex = Math.max(this.selectedIndex - 1, -1);
                    this.updateSelection(items);
                    break;
                case 'Enter':
                    e.preventDefault();
                    if (this.selectedIndex >= 0) {
                        items[this.selectedIndex].click();
                    }
                    break;
                case 'Escape':
                    this.hideResults();
                    break;
            }
        }

        updateSelection(items) {
            items.forEach((item, index) => {
                if (index === this.selectedIndex) {
                    item.classList.add('selected');
                    item.setAttribute('aria-selected', 'true');
                    item.scrollIntoView({ block: 'nearest' });
                } else {
                    item.classList.remove('selected');
                    item.setAttribute('aria-selected', 'false');
                }
            });
        }
    }

    // ===== IMAGE ZOOM =====
    class ImageZoom {
        constructor(options = {}) {
            this.selector = options.selector || '[data-zoom]';
            this.images = document.querySelectorAll(this.selector);
            
            if (this.images.length > 0) {
                this.init();
            }
        }

        init() {
            this.images.forEach(img => {
                img.style.cursor = 'zoom-in';
                img.addEventListener('click', (e) => {
                    this.openLightbox(e.target);
                });
            });

            console.log('✅ Image Zoom initialized');
        }

        openLightbox(img) {
            // Create lightbox
            const lightbox = document.createElement('div');
            lightbox.className = 'image-lightbox';
            lightbox.innerHTML = `
                <div class="lightbox-backdrop"></div>
                <div class="lightbox-content">
                    <button class="lightbox-close" aria-label="Close">
                        <i class="fas fa-times"></i>
                    </button>
                    <img src="${img.src}" alt="${img.alt}" class="lightbox-image">
                    <div class="lightbox-controls">
                        <button class="lightbox-zoom-in" aria-label="Zoom in">
                            <i class="fas fa-search-plus"></i>
                        </button>
                        <button class="lightbox-zoom-out" aria-label="Zoom out">
                            <i class="fas fa-search-minus"></i>
                        </button>
                    </div>
                </div>
            `;

            document.body.appendChild(lightbox);
            document.body.style.overflow = 'hidden';

            // Animate in
            setTimeout(() => lightbox.classList.add('active'), 10);

            // Event listeners
            const close = () => {
                lightbox.classList.remove('active');
                setTimeout(() => {
                    document.body.removeChild(lightbox);
                    document.body.style.overflow = '';
                }, 300);
            };

            lightbox.querySelector('.lightbox-close').addEventListener('click', close);
            lightbox.querySelector('.lightbox-backdrop').addEventListener('click', close);
            
            // ESC key
            const escHandler = (e) => {
                if (e.key === 'Escape') {
                    close();
                    document.removeEventListener('keydown', escHandler);
                }
            };
            document.addEventListener('keydown', escHandler);

            // Zoom controls
            const lightboxImg = lightbox.querySelector('.lightbox-image');
            let scale = 1;

            lightbox.querySelector('.lightbox-zoom-in').addEventListener('click', () => {
                scale = Math.min(scale + 0.25, 3);
                lightboxImg.style.transform = `scale(${scale})`;
            });

            lightbox.querySelector('.lightbox-zoom-out').addEventListener('click', () => {
                scale = Math.max(scale - 0.25, 1);
                lightboxImg.style.transform = `scale(${scale})`;
            });
        }
    }

    // ===== WISHLIST =====
    class Wishlist {
        constructor() {
            this.storageKey = 'coffeehouse-wishlist';
            this.items = this.load();
            this.init();
        }

        init() {
            // Add wishlist buttons to products
            document.querySelectorAll('[data-product-id]').forEach(product => {
                if (!product.querySelector('.wishlist-btn')) {
                    this.addWishlistButton(product);
                }
            });

            // Update wishlist count
            this.updateCount();

            console.log('✅ Wishlist initialized');
        }

        addWishlistButton(product) {
            const productId = product.dataset.productId;
            const isInWishlist = this.items.includes(productId);

            const button = document.createElement('button');
            button.className = 'wishlist-btn';
            button.dataset.productId = productId;
            button.setAttribute('aria-label', isInWishlist ? 'Remove from wishlist' : 'Add to wishlist');
            button.innerHTML = `<i class="fas fa-heart${isInWishlist ? '' : '-o'}"></i>`;

            if (isInWishlist) {
                button.classList.add('active');
            }

            button.addEventListener('click', (e) => {
                e.preventDefault();
                e.stopPropagation();
                this.toggle(productId, button);
            });

            // Add to product hover area
            const hoverArea = product.querySelector('.product__hover ul');
            if (hoverArea) {
                const li = document.createElement('li');
                li.appendChild(button);
                hoverArea.appendChild(li);
            }
        }

        toggle(productId, button) {
            const index = this.items.indexOf(productId);
            
            if (index > -1) {
                // Remove from wishlist
                this.items.splice(index, 1);
                button.classList.remove('active');
                button.querySelector('i').className = 'fas fa-heart-o';
                button.setAttribute('aria-label', 'Add to wishlist');
                
                if (window.toast) {
                    toast.info('Đã xóa khỏi danh sách yêu thích');
                }
            } else {
                // Add to wishlist
                this.items.push(productId);
                button.classList.add('active');
                button.querySelector('i').className = 'fas fa-heart';
                button.setAttribute('aria-label', 'Remove from wishlist');
                
                if (window.toast) {
                    toast.success('Đã thêm vào danh sách yêu thích');
                }
            }

            this.save();
            this.updateCount();
        }

        load() {
            try {
                const data = localStorage.getItem(this.storageKey);
                return data ? JSON.parse(data) : [];
            } catch (e) {
                return [];
            }
        }

        save() {
            try {
                localStorage.setItem(this.storageKey, JSON.stringify(this.items));
            } catch (e) {
                console.error('Failed to save wishlist:', e);
            }
        }

        updateCount() {
            const countElements = document.querySelectorAll('[data-wishlist-count]');
            countElements.forEach(el => {
                el.textContent = this.items.length;
                if (this.items.length > 0) {
                    el.style.display = 'inline-block';
                } else {
                    el.style.display = 'none';
                }
            });
        }

        getItems() {
            return this.items;
        }

        clear() {
            this.items = [];
            this.save();
            this.updateCount();
        }
    }

    // ===== INITIALIZE ALL =====
    function init() {
        // Infinite Scroll
        window.infiniteScroll = new InfiniteScroll();

        // Search Autocomplete
        window.searchAutocomplete = new SearchAutocomplete();

        // Image Zoom
        window.imageZoom = new ImageZoom();

        // Wishlist
        window.wishlist = new Wishlist();

        console.log('✅ Advanced Interactions initialized');
    }

    // Run on DOM ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }

})();
