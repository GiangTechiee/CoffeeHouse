/**
 * Dark Mode Implementation
 * Phase 5: Modern Features
 */

class DarkMode {
    constructor() {
        this.darkModeKey = 'coffeehouse-dark-mode';
        this.init();
    }

    init() {
        // Check saved preference or system preference
        const savedMode = localStorage.getItem(this.darkModeKey);
        const systemPrefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
        
        if (savedMode === 'dark' || (!savedMode && systemPrefersDark)) {
            this.enable();
        } else {
            this.disable();
        }

        // Listen for system preference changes
        window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', (e) => {
            if (!localStorage.getItem(this.darkModeKey)) {
                if (e.matches) {
                    this.enable();
                } else {
                    this.disable();
                }
            }
        });

        // Create toggle button if it doesn't exist
        this.createToggleButton();

        console.log('✅ Dark Mode initialized');
    }

    enable() {
        document.documentElement.classList.add('dark');
        localStorage.setItem(this.darkModeKey, 'dark');
        this.updateToggleButton(true);
        
        // Announce to screen reader
        if (window.announceToScreenReader) {
            window.announceToScreenReader('Đã bật chế độ tối', 'polite');
        }

        // Dispatch event
        window.dispatchEvent(new CustomEvent('darkmodechange', { detail: { enabled: true } }));
    }

    disable() {
        document.documentElement.classList.remove('dark');
        localStorage.setItem(this.darkModeKey, 'light');
        this.updateToggleButton(false);
        
        // Announce to screen reader
        if (window.announceToScreenReader) {
            window.announceToScreenReader('Đã tắt chế độ tối', 'polite');
        }

        // Dispatch event
        window.dispatchEvent(new CustomEvent('darkmodechange', { detail: { enabled: false } }));
    }

    toggle() {
        if (this.isEnabled()) {
            this.disable();
        } else {
            this.enable();
        }
    }

    isEnabled() {
        return document.documentElement.classList.contains('dark');
    }

    createToggleButton() {
        // Check if button already exists
        if (document.getElementById('dark-mode-toggle')) return;

        const button = document.createElement('button');
        button.id = 'dark-mode-toggle';
        button.className = 'dark-mode-toggle';
        button.setAttribute('aria-label', 'Toggle dark mode');
        button.setAttribute('title', 'Toggle dark mode');
        button.innerHTML = `
            <span class="dark-mode-icon light-icon">
                <i class="fas fa-moon"></i>
            </span>
            <span class="dark-mode-icon dark-icon">
                <i class="fas fa-sun"></i>
            </span>
        `;

        button.addEventListener('click', () => {
            this.toggle();
        });

        // Add to header or body
        const header = document.querySelector('.header__right__widget');
        if (header) {
            const li = document.createElement('li');
            li.appendChild(button);
            header.insertBefore(li, header.firstChild);
        } else {
            document.body.appendChild(button);
        }

        this.updateToggleButton(this.isEnabled());
    }

    updateToggleButton(isDark) {
        const button = document.getElementById('dark-mode-toggle');
        if (!button) return;

        const lightIcon = button.querySelector('.light-icon');
        const darkIcon = button.querySelector('.dark-icon');

        if (isDark) {
            lightIcon.style.display = 'none';
            darkIcon.style.display = 'block';
            button.setAttribute('aria-label', 'Tắt chế độ tối');
            button.setAttribute('title', 'Tắt chế độ tối');
        } else {
            lightIcon.style.display = 'block';
            darkIcon.style.display = 'none';
            button.setAttribute('aria-label', 'Bật chế độ tối');
            button.setAttribute('title', 'Bật chế độ tối');
        }
    }
}

// Create global instance
window.darkMode = new DarkMode();

// Export for use in other scripts
window.toggleDarkMode = function() {
    window.darkMode.toggle();
};
