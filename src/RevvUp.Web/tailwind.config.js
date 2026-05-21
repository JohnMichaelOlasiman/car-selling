/** @type {import('tailwindcss').Config} */
module.exports = {
  darkMode: 'class',

  content: [
    './Views/**/*.cshtml',
    './wwwroot/js/**/*.js',
  ],

  theme: {
    extend: {
      colors: {
        // Sophisticated Palette
        primary: {
          50:  '#f0f6ff',
          100: '#e0edff',
          200: '#b8d8ff',
          300: '#7ab8ff',
          400: '#3894ff',
          500: '#3b82f6', // Elegant brand blue
          600: '#2563eb', // Hover state
          700: '#1d4ed8',
          800: '#1e40af',
          900: '#1e3a8a',
          950: '#172554',
        },
        surface: {
          50:  '#f8fafc',
          100: '#f1f5f9',
          200: '#e2e8f0',
          300: '#cbd5e1',
          400: '#94a3b8', // Text secondary
          500: '#64748b',
          600: '#475569',
          700: '#334155',
          800: '#1e293b', // Borders
          850: '#1a1f2e', // Surface elevated
          900: '#131720', // Surface
          950: '#0a0e14', // Base background
        },
      },

      fontFamily: {
        sans:    ['Inter', 'system-ui', '-apple-system', 'sans-serif'],
        display: ['Outfit', 'Inter', 'system-ui', 'sans-serif'],
      },

      boxShadow: {
        'card':       '0 12px 30px -10px rgba(0,0,0,0.4), 0 0 0 1px rgba(30, 41, 59, 0.5)',
        'card-hover': '0 20px 40px -12px rgba(0,0,0,0.6), 0 0 0 1px rgba(59, 130, 246, 0.25)',
        'elevated':   '0 30px 60px -15px rgba(0,0,0,0.6), 0 0 0 1px rgba(30, 41, 59, 0.8)',
      },

      borderRadius: {
        'xl': '0.75rem',
        '2xl': '1rem',
        '3xl': '1.5rem',
      },

      animation: {
        'fade-in':       'fadeIn 300ms cubic-bezier(0.4, 0, 0.2, 1) forwards',
        'fade-in-up':    'fadeInUp 300ms cubic-bezier(0.4, 0, 0.2, 1) forwards',
        'fade-in-down':  'fadeInDown 300ms cubic-bezier(0.4, 0, 0.2, 1) forwards',
        'slide-in-right':'slideInRight 300ms cubic-bezier(0.4, 0, 0.2, 1) forwards',
        'float':         'float 5s ease-in-out infinite',
        'shimmer':       'shimmer 2s linear infinite',
      },

      keyframes: {
        fadeIn: {
          '0%':   { opacity: '0' },
          '100%': { opacity: '1' },
        },
        fadeInUp: {
          '0%':   { opacity: '0', transform: 'translateY(16px)' },
          '100%': { opacity: '1', transform: 'translateY(0)' },
        },
        fadeInDown: {
          '0%':   { opacity: '0', transform: 'translateY(-16px)' },
          '100%': { opacity: '1', transform: 'translateY(0)' },
        },
        slideInRight: {
          '0%':   { opacity: '0', transform: 'translateX(16px)' },
          '100%': { opacity: '1', transform: 'translateX(0)' },
        },
        float: {
          '0%, 100%': { transform: 'translateY(0)' },
          '50%':      { transform: 'translateY(-6px)' },
        },
        shimmer: {
          '0%':   { backgroundPosition: '-200% 0' },
          '100%': { backgroundPosition: '200% 0' },
        },
      },

      spacing: {
        '18': '4.5rem',
        '88': '22rem',
        '128':'32rem',
      },
    },
  },

  plugins: [],
};
