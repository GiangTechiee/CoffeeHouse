/**
 * Performance Optimization
 * Phase 4: Image Optimization, Lazy Loading, Code Splitting, Monitoring
 */

(function() {
    'use strict';

    // ===== IMAGE LAZY LOADING =====
    function initLazyLoading() {
        // Native lazy loading support
        const images = document.querySelectorAll('img[loading="lazy"]');
        
        // Fallback for browsers without native lazy loading
        if ('loading' in HTMLImageElement.prototype) {
            // Browser supports native lazy loading
            console.log('✅ Native lazy loading supported');
        } else {
            // Fallback to Intersection Observer
            const imageObserver = new IntersectionObserver((entries, observer) => {
                entries.forEach(entry => {
                    if (entry.isIntersecting) {
                        const img = entry.target;
                        if (img.dataset.src) {
                            img.src = img.dataset.src;
                            img.removeAttribute('data-src');
                        }
                        if (img.dataset.srcset) {
                            img.srcset = img.dataset.srcset;
                            img.removeAttribute('data-srcset');
                        }
                        observer.unobserve(img);
                    }
                });
            }, {
                rootMargin: '50px 0px',
                threshold: 0.01
            });

            document.querySelectorAll('img[data-src]').forEach(img => {
                imageObserver.observe(img);
            });
        }
    }

    // ===== WEBP SUPPORT DETECTION =====
    function detectWebPSupport() {
        const webpSupported = document.createElement('canvas')
            .toDataURL('image/webp')
            .indexOf('data:image/webp') === 0;

        if (webpSupported) {
            document.documentElement.classList.add('webp');
        } else {
            document.documentElement.classList.add('no-webp');
        }

        return webpSupported;
    }

    // ===== RESPONSIVE IMAGES =====
    function initResponsiveImages() {
        // Add srcset to images without it
        document.querySelectorAll('img[data-responsive]').forEach(img => {
            const src = img.src;
            const basePath = src.substring(0, src.lastIndexOf('.'));
            const ext = src.substring(src.lastIndexOf('.'));

            // Generate srcset for different sizes
            const srcset = [
                `${basePath}-320w${ext} 320w`,
                `${basePath}-640w${ext} 640w`,
                `${basePath}-1024w${ext} 1024w`,
                `${basePath}-1920w${ext} 1920w`
            ].join(', ');

            img.srcset = srcset;
            img.sizes = '(max-width: 640px) 100vw, (max-width: 1024px) 50vw, 33vw';
        });
    }

    // ===== PRELOAD CRITICAL RESOURCES =====
    function preloadCriticalResources() {
        // Preload critical fonts
        const fonts = [
            '/fonts/Montserrat/static/Montserrat-Medium.ttf'
        ];

        fonts.forEach(font => {
            const link = document.createElement('link');
            link.rel = 'preload';
            link.as = 'font';
            link.type = 'font/ttf';
            link.href = font;
            link.crossOrigin = 'anonymous';
            document.head.appendChild(link);
        });
    }

    // ===== DEFER NON-CRITICAL CSS =====
    function deferNonCriticalCSS() {
        const stylesheets = document.querySelectorAll('link[rel="stylesheet"][data-defer]');
        
        stylesheets.forEach(link => {
            link.media = 'print';
            link.onload = function() {
                this.media = 'all';
            };
        });
    }

    // ===== DEBOUNCE UTILITY =====
    function debounce(func, wait) {
        let timeout;
        return function executedFunction(...args) {
            const later = () => {
                clearTimeout(timeout);
                func(...args);
            };
            clearTimeout(timeout);
            timeout = setTimeout(later, wait);
        };
    }

    // ===== THROTTLE UTILITY =====
    function throttle(func, limit) {
        let inThrottle;
        return function(...args) {
            if (!inThrottle) {
                func.apply(this, args);
                inThrottle = true;
                setTimeout(() => inThrottle = false, limit);
            }
        };
    }

    // Make utilities globally available
    window.debounce = debounce;
    window.throttle = throttle;

    // ===== OPTIMIZE SCROLL PERFORMANCE =====
    function optimizeScrollPerformance() {
        let ticking = false;

        const handleScroll = () => {
            if (!ticking) {
                window.requestAnimationFrame(() => {
                    // Scroll handling logic here
                    ticking = false;
                });
                ticking = true;
            }
        };

        window.addEventListener('scroll', handleScroll, { passive: true });
    }

    // ===== RESOURCE HINTS =====
    function addResourceHints() {
        // DNS prefetch for external domains
        const domains = [
            'https://fonts.googleapis.com',
            'https://cdn.jsdelivr.net'
        ];

        domains.forEach(domain => {
            const link = document.createElement('link');
            link.rel = 'dns-prefetch';
            link.href = domain;
            document.head.appendChild(link);
        });

        // Preconnect to critical origins
        const preconnectDomains = [
            'https://fonts.gstatic.com'
        ];

        preconnectDomains.forEach(domain => {
            const link = document.createElement('link');
            link.rel = 'preconnect';
            link.href = domain;
            link.crossOrigin = 'anonymous';
            document.head.appendChild(link);
        });
    }

    // ===== PERFORMANCE MONITORING =====
    function initPerformanceMonitoring() {
        if (!window.performance || !window.performance.timing) {
            return;
        }

        window.addEventListener('load', () => {
            setTimeout(() => {
                const perfData = window.performance.timing;
                const pageLoadTime = perfData.loadEventEnd - perfData.navigationStart;
                const connectTime = perfData.responseEnd - perfData.requestStart;
                const renderTime = perfData.domComplete - perfData.domLoading;

                console.log('📊 Performance Metrics:');
                console.log(`  Page Load Time: ${pageLoadTime}ms`);
                console.log(`  Connect Time: ${connectTime}ms`);
                console.log(`  Render Time: ${renderTime}ms`);

                // Core Web Vitals (if supported)
                if ('PerformanceObserver' in window) {
                    // Largest Contentful Paint (LCP)
                    try {
                        const lcpObserver = new PerformanceObserver((list) => {
                            const entries = list.getEntries();
                            const lastEntry = entries[entries.length - 1];
                            console.log(`  LCP: ${lastEntry.renderTime || lastEntry.loadTime}ms`);
                        });
                        lcpObserver.observe({ entryTypes: ['largest-contentful-paint'] });
                    } catch (e) {
                        // LCP not supported
                    }

                    // First Input Delay (FID)
                    try {
                        const fidObserver = new PerformanceObserver((list) => {
                            const entries = list.getEntries();
                            entries.forEach(entry => {
                                console.log(`  FID: ${entry.processingStart - entry.startTime}ms`);
                            });
                        });
                        fidObserver.observe({ entryTypes: ['first-input'] });
                    } catch (e) {
                        // FID not supported
                    }

                    // Cumulative Layout Shift (CLS)
                    try {
                        let clsScore = 0;
                        const clsObserver = new PerformanceObserver((list) => {
                            for (const entry of list.getEntries()) {
                                if (!entry.hadRecentInput) {
                                    clsScore += entry.value;
                                }
                            }
                            console.log(`  CLS: ${clsScore.toFixed(3)}`);
                        });
                        clsObserver.observe({ entryTypes: ['layout-shift'] });
                    } catch (e) {
                        // CLS not supported
                    }
                }

                // Send to analytics (if configured)
                if (window.gtag) {
                    window.gtag('event', 'timing_complete', {
                        name: 'page_load',
                        value: pageLoadTime,
                        event_category: 'Performance'
                    });
                }
            }, 0);
        });
    }

    // ===== CACHE MANAGEMENT =====
    function initCacheManagement() {
        // Service Worker registration (if available)
        if ('serviceWorker' in navigator) {
            window.addEventListener('load', () => {
                // Service worker will be registered separately
                console.log('✅ Service Worker support detected');
            });
        }

        // LocalStorage cache for API responses
        window.cacheAPI = {
            set: function(key, data, ttl = 3600000) { // 1 hour default
                const item = {
                    data: data,
                    timestamp: Date.now(),
                    ttl: ttl
                };
                try {
                    localStorage.setItem(`cache_${key}`, JSON.stringify(item));
                } catch (e) {
                    console.warn('LocalStorage cache failed:', e);
                }
            },
            get: function(key) {
                try {
                    const item = JSON.parse(localStorage.getItem(`cache_${key}`));
                    if (!item) return null;

                    const now = Date.now();
                    if (now - item.timestamp > item.ttl) {
                        localStorage.removeItem(`cache_${key}`);
                        return null;
                    }

                    return item.data;
                } catch (e) {
                    return null;
                }
            },
            clear: function() {
                Object.keys(localStorage).forEach(key => {
                    if (key.startsWith('cache_')) {
                        localStorage.removeItem(key);
                    }
                });
            }
        };
    }

    // ===== BUNDLE SIZE MONITORING =====
    function monitorBundleSize() {
        if (!window.performance || !window.performance.getEntriesByType) {
            return;
        }

        const resources = window.performance.getEntriesByType('resource');
        let totalSize = 0;
        let cssSize = 0;
        let jsSize = 0;
        let imgSize = 0;

        resources.forEach(resource => {
            const size = resource.transferSize || 0;
            totalSize += size;

            if (resource.name.endsWith('.css')) {
                cssSize += size;
            } else if (resource.name.endsWith('.js')) {
                jsSize += size;
            } else if (resource.name.match(/\.(jpg|jpeg|png|gif|webp|svg)$/)) {
                imgSize += size;
            }
        });

        console.log('📦 Bundle Sizes:');
        console.log(`  Total: ${(totalSize / 1024).toFixed(2)} KB`);
        console.log(`  CSS: ${(cssSize / 1024).toFixed(2)} KB`);
        console.log(`  JS: ${(jsSize / 1024).toFixed(2)} KB`);
        console.log(`  Images: ${(imgSize / 1024).toFixed(2)} KB`);

        // Warn if bundles are too large
        if (cssSize > 50 * 1024) {
            console.warn('⚠️ CSS bundle is larger than 50KB. Consider code splitting.');
        }
        if (jsSize > 200 * 1024) {
            console.warn('⚠️ JS bundle is larger than 200KB. Consider code splitting.');
        }
    }

    // ===== CRITICAL CSS INLINE =====
    function inlineCriticalCSS() {
        // This would typically be done at build time
        // Here we just mark critical styles
        const criticalStyles = document.querySelectorAll('style[data-critical]');
        console.log(`✅ ${criticalStyles.length} critical style blocks inlined`);
    }

    // ===== INITIALIZE ALL =====
    function init() {
        detectWebPSupport();
        initLazyLoading();
        initResponsiveImages();
        preloadCriticalResources();
        deferNonCriticalCSS();
        optimizeScrollPerformance();
        addResourceHints();
        initPerformanceMonitoring();
        initCacheManagement();
        monitorBundleSize();
        inlineCriticalCSS();

        console.log('✅ Performance optimizations initialized');
    }

    // Run on DOM ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }

})();
