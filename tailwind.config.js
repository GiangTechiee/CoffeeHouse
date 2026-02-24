/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    './src/CoffeeHouse.Web/Views/**/*.cshtml',
    './src/CoffeeHouse.Web/wwwroot/js/**/*.js', 
    './src/CoffeeHouse.Web/Areas/**/*.cshtml'
  ],
  theme: {
    extend: {
      colors: {
        primary: {
          DEFAULT: "#ec8e13",
          dark: "#d47f11",
        },
        "background-light": "#FCFAF8",
        "background-dark": "#1a1612",
        "beige-soft": "#F3EEEA",
        "moss-green": "#4A5D4E",
        "warm-gray": "#897861",
        secondary: '#111111',
        accent: '#36a300',
        surface: {
          DEFAULT: '#ffffff',
          dark: '#f4f4f4',
          darker: '#e1e1e1',
        },
        text: {
          DEFAULT: '#111111',
          muted: '#666666',
          light: '#888888',
        },
      },
      fontFamily: {
        display: ["Work Sans", "sans-serif"],
        serif: ["Playfair Display", "serif"],
        sans: ['Montserrat', 'system-ui', '-apple-system', 'sans-serif'],
      },
      spacing: {
        'xs': '0.25rem',
        'sm': '0.5rem',
        'md': '1rem',
        'lg': '2rem',
        'xl': '4rem',
        '2xl': '6rem',
      },
      boxShadow: {
        'sm': '0 1px 2px 0 rgba(0, 0, 0, 0.05)',
        'md': '0 4px 6px -1px rgba(0, 0, 0, 0.1)',
        'lg': '0 10px 15px -3px rgba(0, 0, 0, 0.1)',
        'xl': '0 20px 25px -5px rgba(0, 0, 0, 0.1)',
      },
    },
  },
  plugins: [],
  // Performance optimizations
  corePlugins: {
    // Disable unused core plugins to reduce bundle size
    preflight: true,
  },
  // Safelist important classes that might be added dynamically
  safelist: [
    'active',
    'show',
    'hidden',
    'fade-in',
    'slide-up',
    'scale-in',
    'btn-loading',
    'error',
    'success',
    'warning',
    'info',
  ],
}

