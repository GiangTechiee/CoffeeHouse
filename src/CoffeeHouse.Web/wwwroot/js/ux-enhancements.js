/**
 * UX Enhancements
 * Phase 2: Loading States, Animations, and Interactions
 */

(function() {
    'use strict';

    // ===== STICKY HEADER WITH SHADOW ON SCROLL =====
    function initStickyHeader() {
        const header = document.querySelector('.header');
        if (!header) return;

        let lastScroll = 0;

        window.addEventListener('scroll', () => {
            const currentScroll = window.pageYOffset;

            if (currentScroll > 100) {
                header.classList.add('scrolled');
            } else {
                header.classList.remove('scrolled');
            }

            lastScroll = currentScroll;
        });
    }

    // ===== SMOOTH SCROLL BEHAVIOR =====
    function initSmoothScroll() {
        document.querySelectorAll('a[href^="#"]').forEach(anchor => {
            anchor.addEventListener('click', function(e) {
                const href = this.getAttribute('href');
                if (href === '#' || href === '#!') return;

                const target = document.querySelector(href);
                if (target) {
                    e.preventDefault();
                    target.scrollIntoView({
                        behavior: 'smooth',
                        block: 'start'
                    });
                }
            });
        });
    }

    // ===== LOADING BUTTON STATE =====
    function initLoadingButtons() {
        // Add loading state to buttons
        window.setButtonLoading = function(button, loading = true) {
            if (loading) {
                button.classList.add('btn-loading');
                button.disabled = true;
                button.dataset.originalText = button.textContent;
                button.textContent = 'Đang xử lý...';
            } else {
                button.classList.remove('btn-loading');
                button.disabled = false;
                if (button.dataset.originalText) {
                    button.textContent = button.dataset.originalText;
                }
            }
        };
    }

    // ===== ADD TO CART WITH FEEDBACK =====
    function initAddToCartFeedback() {
        document.addEventListener('click', function(e) {
            const addToCartBtn = e.target.closest('.add-to-cart');
            if (!addToCartBtn) return;

            e.preventDefault();

            // Show loading state
            const originalHtml = addToCartBtn.innerHTML;
            addToCartBtn.innerHTML = '<i class="fas fa-spinner fa-spin"></i>';
            addToCartBtn.disabled = true;

            // Simulate API call (replace with actual AJAX)
            setTimeout(() => {
                // Success feedback
                addToCartBtn.innerHTML = '<i class="fas fa-check"></i> Đã thêm';
                addToCartBtn.classList.add('success');

                // Show toast
                if (window.toast) {
                    toast.success('Đã thêm sản phẩm vào giỏ hàng!');
                }

                // Reset button after 2s
                setTimeout(() => {
                    addToCartBtn.innerHTML = originalHtml;
                    addToCartBtn.disabled = false;
                    addToCartBtn.classList.remove('success');
                }, 2000);
            }, 800);
        });
    }

    // ===== FORM VALIDATION FEEDBACK =====
    function initFormValidation() {
        const forms = document.querySelectorAll('form[data-validate]');

        forms.forEach(form => {
            form.addEventListener('submit', function(e) {
                let isValid = true;

                // Validate required fields
                const requiredFields = form.querySelectorAll('[required]');
                requiredFields.forEach(field => {
                    if (!field.value.trim()) {
                        isValid = false;
                        showFieldError(field, 'Trường này là bắt buộc');
                    } else {
                        clearFieldError(field);
                    }
                });

                // Validate email
                const emailFields = form.querySelectorAll('input[type="email"]');
                emailFields.forEach(field => {
                    if (field.value && !isValidEmail(field.value)) {
                        isValid = false;
                        showFieldError(field, 'Email không hợp lệ');
                    }
                });

                if (!isValid) {
                    e.preventDefault();
                }
            });

            // Real-time validation
            const inputs = form.querySelectorAll('input, textarea, select');
            inputs.forEach(input => {
                input.addEventListener('blur', function() {
                    if (this.hasAttribute('required') && !this.value.trim()) {
                        showFieldError(this, 'Trường này là bắt buộc');
                    } else if (this.type === 'email' && this.value && !isValidEmail(this.value)) {
                        showFieldError(this, 'Email không hợp lệ');
                    } else {
                        clearFieldError(this);
                    }
                });

                input.addEventListener('input', function() {
                    if (this.classList.contains('error')) {
                        clearFieldError(this);
                    }
                });
            });
        });
    }

    function showFieldError(field, message) {
        field.classList.add('error');
        field.classList.remove('success');

        // Remove existing error message
        const existingError = field.parentNode.querySelector('.form-error');
        if (existingError) {
            existingError.remove();
        }

        // Add error message
        const errorDiv = document.createElement('div');
        errorDiv.className = 'form-error';
        errorDiv.textContent = message;
        field.parentNode.appendChild(errorDiv);
    }

    function clearFieldError(field) {
        field.classList.remove('error');
        field.classList.add('success');

        const errorDiv = field.parentNode.querySelector('.form-error');
        if (errorDiv) {
            errorDiv.remove();
        }
    }

    function isValidEmail(email) {
        return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);
    }

    // ===== SKELETON LOADER =====
    function showSkeletonLoader(container, count = 4) {
        const skeletonHTML = `
            <div class="skeleton-card">
                <div class="skeleton skeleton-image"></div>
                <div class="skeleton skeleton-title"></div>
                <div class="skeleton skeleton-price"></div>
            </div>
        `;

        container.innerHTML = skeletonHTML.repeat(count);
    }

    window.showSkeletonLoader = showSkeletonLoader;

    // ===== FADE IN ON SCROLL =====
    function initScrollAnimations() {
        const observerOptions = {
            threshold: 0.1,
            rootMargin: '0px 0px -50px 0px'
        };

        const observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    entry.target.classList.add('fade-in');
                    observer.unobserve(entry.target);
                }
            });
        }, observerOptions);

        // Observe elements with data-animate attribute
        document.querySelectorAll('[data-animate]').forEach(el => {
            observer.observe(el);
        });
    }

    // ===== IMAGE LAZY LOADING =====
    function initLazyLoading() {
        const images = document.querySelectorAll('img[data-src]');

        const imageObserver = new IntersectionObserver((entries, observer) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    const img = entry.target;
                    img.src = img.dataset.src;
                    img.classList.add('fade-in');
                    img.removeAttribute('data-src');
                    observer.unobserve(img);
                }
            });
        });

        images.forEach(img => imageObserver.observe(img));
    }

    // ===== SEARCH BAR ANIMATION =====
    function initSearchAnimation() {
        const searchSwitch = document.querySelector('.search-switch');
        const searchBox = document.querySelector('.search-model');

        if (searchSwitch && searchBox) {
            searchSwitch.addEventListener('click', function() {
                searchBox.classList.add('fade-in');
            });
        }
    }

    // ===== MOBILE MENU =====
    function initMobileMenu() {
        const menuToggle = document.querySelector('.canvas__open');
        const menu = document.querySelector('.header__menu');

        if (menuToggle && menu) {
            menuToggle.addEventListener('click', function() {
                menu.classList.toggle('active');
                this.querySelector('i').classList.toggle('fa-bars');
                this.querySelector('i').classList.toggle('fa-times');
            });
        }
    }

    // ===== CONFIRMATION DIALOGS =====
    window.confirmAction = function(message, callback) {
        if (confirm(message)) {
            callback();
        }
    };

    // ===== BACK TO TOP BUTTON =====
    function initBackToTop() {
        const backToTop = document.querySelector('.back-to-top');
        if (!backToTop) return;

        window.addEventListener('scroll', () => {
            if (window.pageYOffset > 300) {
                backToTop.style.display = 'block';
                backToTop.classList.add('fade-in');
            } else {
                backToTop.style.display = 'none';
            }
        });

        backToTop.addEventListener('click', (e) => {
            e.preventDefault();
            window.scrollTo({
                top: 0,
                behavior: 'smooth'
            });
        });
    }

    // ===== INITIALIZE ALL =====
    function init() {
        initStickyHeader();
        initSmoothScroll();
        initLoadingButtons();
        initAddToCartFeedback();
        initFormValidation();
        initScrollAnimations();
        initLazyLoading();
        initSearchAnimation();
        initMobileMenu();
        initBackToTop();

        console.log('✅ UX Enhancements initialized');
    }

    // Run on DOM ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }

})();
