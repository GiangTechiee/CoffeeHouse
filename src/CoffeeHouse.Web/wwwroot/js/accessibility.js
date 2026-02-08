/**
 * Accessibility Enhancements - WCAG 2.1 AA Compliance
 * Phase 3: Keyboard Navigation, Focus Management, Screen Reader Support
 */

(function() {
    'use strict';

    // ===== SKIP TO CONTENT LINK =====
    function initSkipToContent() {
        // Create skip link if it doesn't exist
        if (!document.querySelector('.skip-to-content')) {
            const skipLink = document.createElement('a');
            skipLink.href = '#main-content';
            skipLink.className = 'skip-to-content';
            skipLink.textContent = 'Bỏ qua đến nội dung chính';
            skipLink.setAttribute('tabindex', '0');
            
            document.body.insertBefore(skipLink, document.body.firstChild);
            
            skipLink.addEventListener('click', function(e) {
                e.preventDefault();
                const mainContent = document.getElementById('main-content') || document.querySelector('main');
                if (mainContent) {
                    mainContent.setAttribute('tabindex', '-1');
                    mainContent.focus();
                    mainContent.scrollIntoView({ behavior: 'smooth' });
                }
            });
        }
    }

    // ===== KEYBOARD NAVIGATION =====
    function initKeyboardNavigation() {
        // ESC key to close modals and dropdowns
        document.addEventListener('keydown', function(e) {
            if (e.key === 'Escape' || e.keyCode === 27) {
                // Close all modals
                const modals = document.querySelectorAll('.modal.show, .search-model.active');
                modals.forEach(modal => {
                    modal.classList.remove('show', 'active');
                    // Return focus to trigger element
                    const trigger = modal.dataset.trigger;
                    if (trigger) {
                        document.querySelector(trigger)?.focus();
                    }
                });

                // Close all dropdowns
                const dropdowns = document.querySelectorAll('.custom-dropdown.active');
                dropdowns.forEach(dropdown => {
                    dropdown.classList.remove('active');
                });

                // Close toast notifications
                const toasts = document.querySelectorAll('.toast');
                toasts.forEach(toast => {
                    if (window.toast && window.toast.dismiss) {
                        window.toast.dismiss(toast);
                    }
                });
            }
        });

        // Enter key to submit forms and activate buttons
        document.addEventListener('keydown', function(e) {
            if (e.key === 'Enter' || e.keyCode === 13) {
                const target = e.target;
                
                // If on a button or link, activate it
                if (target.tagName === 'BUTTON' || target.tagName === 'A') {
                    if (!target.disabled) {
                        target.click();
                    }
                }
            }
        });

        // Arrow keys for dropdown navigation
        document.querySelectorAll('.custom-dropdown').forEach(dropdown => {
            const menu = dropdown.querySelector('.dropdown-menu');
            const items = menu?.querySelectorAll('.dropdown-item');
            
            if (!items || items.length === 0) return;

            let currentIndex = -1;

            dropdown.addEventListener('keydown', function(e) {
                if (!dropdown.classList.contains('active')) return;

                if (e.key === 'ArrowDown' || e.keyCode === 40) {
                    e.preventDefault();
                    currentIndex = (currentIndex + 1) % items.length;
                    items[currentIndex].focus();
                } else if (e.key === 'ArrowUp' || e.keyCode === 38) {
                    e.preventDefault();
                    currentIndex = currentIndex <= 0 ? items.length - 1 : currentIndex - 1;
                    items[currentIndex].focus();
                } else if (e.key === 'Home' || e.keyCode === 36) {
                    e.preventDefault();
                    currentIndex = 0;
                    items[currentIndex].focus();
                } else if (e.key === 'End' || e.keyCode === 35) {
                    e.preventDefault();
                    currentIndex = items.length - 1;
                    items[currentIndex].focus();
                }
            });
        });
    }

    // ===== FOCUS MANAGEMENT =====
    function initFocusManagement() {
        // Focus trap for modals
        function trapFocus(element) {
            const focusableElements = element.querySelectorAll(
                'a[href], button:not([disabled]), textarea:not([disabled]), input:not([disabled]), select:not([disabled]), [tabindex]:not([tabindex="-1"])'
            );
            
            if (focusableElements.length === 0) return;

            const firstFocusable = focusableElements[0];
            const lastFocusable = focusableElements[focusableElements.length - 1];

            element.addEventListener('keydown', function(e) {
                if (e.key !== 'Tab' && e.keyCode !== 9) return;

                if (e.shiftKey) {
                    if (document.activeElement === firstFocusable) {
                        e.preventDefault();
                        lastFocusable.focus();
                    }
                } else {
                    if (document.activeElement === lastFocusable) {
                        e.preventDefault();
                        firstFocusable.focus();
                    }
                }
            });

            // Focus first element when modal opens
            firstFocusable.focus();
        }

        // Apply focus trap to modals
        const observer = new MutationObserver(function(mutations) {
            mutations.forEach(function(mutation) {
                mutation.addedNodes.forEach(function(node) {
                    if (node.nodeType === 1 && node.classList?.contains('modal')) {
                        trapFocus(node);
                    }
                });
            });
        });

        observer.observe(document.body, { childList: true, subtree: true });

        // Existing modals
        document.querySelectorAll('.modal, .search-model').forEach(modal => {
            modal.addEventListener('shown', function() {
                trapFocus(this);
            });
        });
    }

    // ===== FOCUS VISIBLE (KEYBOARD ONLY) =====
    function initFocusVisible() {
        let isUsingKeyboard = false;

        document.addEventListener('keydown', function(e) {
            if (e.key === 'Tab' || e.keyCode === 9) {
                isUsingKeyboard = true;
                document.body.classList.add('keyboard-navigation');
            }
        });

        document.addEventListener('mousedown', function() {
            isUsingKeyboard = false;
            document.body.classList.remove('keyboard-navigation');
        });
    }

    // ===== ARIA LIVE REGIONS =====
    function initAriaLiveRegions() {
        // Create live region for announcements if it doesn't exist
        if (!document.getElementById('aria-live-region')) {
            const liveRegion = document.createElement('div');
            liveRegion.id = 'aria-live-region';
            liveRegion.className = 'sr-only';
            liveRegion.setAttribute('aria-live', 'polite');
            liveRegion.setAttribute('aria-atomic', 'true');
            document.body.appendChild(liveRegion);
        }

        // Global function to announce to screen readers
        window.announceToScreenReader = function(message, priority = 'polite') {
            const liveRegion = document.getElementById('aria-live-region');
            if (liveRegion) {
                liveRegion.setAttribute('aria-live', priority);
                liveRegion.textContent = message;
                
                // Clear after announcement
                setTimeout(() => {
                    liveRegion.textContent = '';
                }, 1000);
            }
        };
    }

    // ===== FORM ACCESSIBILITY =====
    function initFormAccessibility() {
        // Associate labels with inputs
        document.querySelectorAll('input, select, textarea').forEach(input => {
            const id = input.id || `input-${Math.random().toString(36).substr(2, 9)}`;
            input.id = id;

            // Find associated label
            let label = document.querySelector(`label[for="${id}"]`);
            if (!label) {
                label = input.closest('label');
            }
            if (!label) {
                label = input.previousElementSibling;
                if (label && label.tagName === 'LABEL') {
                    label.setAttribute('for', id);
                }
            }

            // Add aria-required for required fields
            if (input.hasAttribute('required')) {
                input.setAttribute('aria-required', 'true');
            }

            // Add aria-invalid for error states
            input.addEventListener('invalid', function() {
                this.setAttribute('aria-invalid', 'true');
            });

            input.addEventListener('input', function() {
                if (this.validity.valid) {
                    this.removeAttribute('aria-invalid');
                }
            });
        });

        // Error message association
        document.querySelectorAll('.form-error').forEach(error => {
            const input = error.previousElementSibling;
            if (input && (input.tagName === 'INPUT' || input.tagName === 'TEXTAREA' || input.tagName === 'SELECT')) {
                const errorId = `error-${input.id || Math.random().toString(36).substr(2, 9)}`;
                error.id = errorId;
                input.setAttribute('aria-describedby', errorId);
            }
        });
    }

    // ===== BUTTON ACCESSIBILITY =====
    function initButtonAccessibility() {
        // Add aria-label to icon-only buttons
        document.querySelectorAll('button, a').forEach(button => {
            const hasText = button.textContent.trim().length > 0;
            const hasAriaLabel = button.hasAttribute('aria-label') || button.hasAttribute('aria-labelledby');
            const hasIcon = button.querySelector('i, svg');

            if (!hasText && hasIcon && !hasAriaLabel) {
                // Try to infer label from title or class
                const title = button.getAttribute('title');
                if (title) {
                    button.setAttribute('aria-label', title);
                } else {
                    console.warn('Button without accessible label:', button);
                }
            }

            // Add role="button" to links that act as buttons
            if (button.tagName === 'A' && button.getAttribute('role') !== 'button') {
                const href = button.getAttribute('href');
                if (href === '#' || href === '#!' || button.onclick) {
                    button.setAttribute('role', 'button');
                }
            }
        });
    }

    // ===== IMAGE ACCESSIBILITY =====
    function initImageAccessibility() {
        document.querySelectorAll('img').forEach(img => {
            // Check if image has alt text
            if (!img.hasAttribute('alt')) {
                // If decorative, add empty alt
                if (img.closest('.decoration, .background')) {
                    img.setAttribute('alt', '');
                } else {
                    console.warn('Image without alt text:', img.src);
                    img.setAttribute('alt', 'Image'); // Fallback
                }
            }
        });
    }

    // ===== HEADING HIERARCHY =====
    function validateHeadingHierarchy() {
        const headings = document.querySelectorAll('h1, h2, h3, h4, h5, h6');
        let previousLevel = 0;

        headings.forEach(heading => {
            const level = parseInt(heading.tagName.substring(1));
            
            if (previousLevel > 0 && level > previousLevel + 1) {
                console.warn(`Heading hierarchy skip: ${heading.tagName} after h${previousLevel}`, heading);
            }

            previousLevel = level;
        });
    }

    // ===== CONTRAST CHECKER (DEV MODE) =====
    function checkColorContrast() {
        if (window.location.hostname !== 'localhost' && !window.location.hostname.includes('127.0.0.1')) {
            return; // Only run in development
        }

        // This is a simplified checker - use browser extensions for full testing
        console.log('💡 Accessibility Tip: Use browser extensions like axe DevTools or WAVE for comprehensive contrast checking');
    }

    // ===== LANDMARK ROLES =====
    function initLandmarkRoles() {
        // Add main role if not present
        const main = document.querySelector('main');
        if (main && !main.hasAttribute('role')) {
            main.setAttribute('role', 'main');
        }

        // Add navigation role
        document.querySelectorAll('nav').forEach(nav => {
            if (!nav.hasAttribute('role')) {
                nav.setAttribute('role', 'navigation');
            }
            if (!nav.hasAttribute('aria-label')) {
                nav.setAttribute('aria-label', 'Main navigation');
            }
        });

        // Add complementary role to sidebars
        document.querySelectorAll('aside').forEach(aside => {
            if (!aside.hasAttribute('role')) {
                aside.setAttribute('role', 'complementary');
            }
        });

        // Add contentinfo role to footer
        document.querySelectorAll('footer').forEach(footer => {
            if (!footer.hasAttribute('role')) {
                footer.setAttribute('role', 'contentinfo');
            }
        });
    }

    // ===== INITIALIZE ALL =====
    function init() {
        initSkipToContent();
        initKeyboardNavigation();
        initFocusManagement();
        initFocusVisible();
        initAriaLiveRegions();
        initFormAccessibility();
        initButtonAccessibility();
        initImageAccessibility();
        initLandmarkRoles();
        validateHeadingHierarchy();
        checkColorContrast();

        console.log('✅ Accessibility features initialized (WCAG 2.1 AA)');
    }

    // Run on DOM ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }

})();
