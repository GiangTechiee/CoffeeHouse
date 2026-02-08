/**
 * Image Optimization Helper
 * Phase 4: WebP conversion, responsive images, lazy loading
 */

class ImageOptimizer {
    constructor() {
        this.webpSupported = this.detectWebPSupport();
        this.init();
    }

    // Detect WebP support
    detectWebPSupport() {
        const canvas = document.createElement('canvas');
        if (canvas.getContext && canvas.getContext('2d')) {
            return canvas.toDataURL('image/webp').indexOf('data:image/webp') === 0;
        }
        return false;
    }

    // Convert image URL to WebP if supported
    toWebP(url) {
        if (!this.webpSupported) return url;
        
        // Check if already WebP
        if (url.endsWith('.webp')) return url;
        
        // Replace extension with .webp
        return url.replace(/\.(jpg|jpeg|png)$/i, '.webp');
    }

    // Generate srcset for responsive images
    generateSrcSet(baseUrl, sizes = [320, 640, 1024, 1920]) {
        const ext = baseUrl.substring(baseUrl.lastIndexOf('.'));
        const base = baseUrl.substring(0, baseUrl.lastIndexOf('.'));
        
        return sizes.map(size => {
            const url = `${base}-${size}w${ext}`;
            return `${this.toWebP(url)} ${size}w`;
        }).join(', ');
    }

    // Optimize all images on the page
    optimizeImages() {
        document.querySelectorAll('img:not([data-optimized])').forEach(img => {
            // Add loading="lazy" if not present
            if (!img.hasAttribute('loading')) {
                img.setAttribute('loading', 'lazy');
            }

            // Add decoding="async" for better performance
            if (!img.hasAttribute('decoding')) {
                img.setAttribute('decoding', 'async');
            }

            // Generate srcset if image has data-responsive attribute
            if (img.hasAttribute('data-responsive')) {
                const srcset = this.generateSrcSet(img.src);
                img.srcset = srcset;
                img.sizes = img.getAttribute('data-sizes') || '(max-width: 640px) 100vw, (max-width: 1024px) 50vw, 33vw';
            }

            // Convert to WebP if supported
            if (this.webpSupported && img.hasAttribute('data-webp')) {
                img.src = this.toWebP(img.src);
            }

            // Mark as optimized
            img.setAttribute('data-optimized', 'true');
        });
    }

    // Preload critical images
    preloadImage(url, as = 'image') {
        const link = document.createElement('link');
        link.rel = 'preload';
        link.as = as;
        link.href = this.toWebP(url);
        document.head.appendChild(link);
    }

    // Lazy load background images
    lazyLoadBackgrounds() {
        const bgObserver = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    const element = entry.target;
                    const bgUrl = element.dataset.bg;
                    
                    if (bgUrl) {
                        element.style.backgroundImage = `url(${this.toWebP(bgUrl)})`;
                        element.removeAttribute('data-bg');
                        bgObserver.unobserve(element);
                    }
                }
            });
        }, {
            rootMargin: '50px'
        });

        document.querySelectorAll('[data-bg]').forEach(el => {
            bgObserver.observe(el);
        });
    }

    // Compress image quality for thumbnails
    compressThumbnail(img, quality = 0.8) {
        const canvas = document.createElement('canvas');
        const ctx = canvas.getContext('2d');
        
        canvas.width = img.naturalWidth;
        canvas.height = img.naturalHeight;
        
        ctx.drawImage(img, 0, 0);
        
        return canvas.toDataURL('image/jpeg', quality);
    }

    // Get optimal image format
    getOptimalFormat(url) {
        if (this.webpSupported) return 'webp';
        if (url.match(/\.png$/i)) return 'png';
        return 'jpeg';
    }

    // Calculate image dimensions for responsive loading
    calculateOptimalSize(containerWidth, devicePixelRatio = window.devicePixelRatio || 1) {
        const sizes = [320, 640, 1024, 1920];
        const targetWidth = containerWidth * devicePixelRatio;
        
        // Find the smallest size that's larger than target
        for (let size of sizes) {
            if (size >= targetWidth) {
                return size;
            }
        }
        
        return sizes[sizes.length - 1];
    }

    // Monitor image loading performance
    monitorImagePerformance() {
        if (!window.PerformanceObserver) return;

        const imageObserver = new PerformanceObserver((list) => {
            const entries = list.getEntries();
            entries.forEach(entry => {
                if (entry.initiatorType === 'img') {
                    const loadTime = entry.responseEnd - entry.startTime;
                    const size = entry.transferSize;
                    
                    if (loadTime > 1000) {
                        console.warn(`Slow image load: ${entry.name} (${loadTime.toFixed(0)}ms, ${(size/1024).toFixed(1)}KB)`);
                    }
                }
            });
        });

        imageObserver.observe({ entryTypes: ['resource'] });
    }

    // Initialize
    init() {
        // Wait for DOM to be ready
        if (document.readyState === 'loading') {
            document.addEventListener('DOMContentLoaded', () => {
                this.optimizeImages();
                this.lazyLoadBackgrounds();
                this.monitorImagePerformance();
            });
        } else {
            this.optimizeImages();
            this.lazyLoadBackgrounds();
            this.monitorImagePerformance();
        }

        // Re-optimize when new images are added
        const observer = new MutationObserver(() => {
            this.optimizeImages();
        });

        observer.observe(document.body, {
            childList: true,
            subtree: true
        });

        console.log(`✅ Image Optimizer initialized (WebP: ${this.webpSupported ? 'Yes' : 'No'})`);
    }
}

// Create global instance
window.imageOptimizer = new ImageOptimizer();

// Export helper functions
window.optimizeImage = function(url) {
    return window.imageOptimizer.toWebP(url);
};

window.preloadImage = function(url) {
    window.imageOptimizer.preloadImage(url);
};
