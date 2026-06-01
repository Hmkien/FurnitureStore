/** @type {import('tailwindcss').Config} */
export default {
  content: ['./index.html', './src/**/*.{js,ts,jsx,tsx}'],
  theme: {
    extend: {
      colors: {
        cream: '#f6f1e9',
        paper: '#fdfbf7',
        line: '#e7ddcf',
        ink: {
          DEFAULT: '#2a211b',
          soft: '#574b41',
          muted: '#8d8175',
        },
        // Đất nung / clay — màu nhấn chủ đạo
        clay: {
          50: '#fbf2ec',
          100: '#f4ddcf',
          200: '#e8bca2',
          300: '#db9772',
          400: '#cd784e',
          500: '#bd5d38',
          600: '#a44a2b',
          700: '#833924',
          800: '#5f2b1d',
          900: '#3f1e15',
        },
        // alias để tương thích code cũ
        brand: {
          50: '#fbf2ec',
          100: '#f4ddcf',
          500: '#bd5d38',
          600: '#a44a2b',
          700: '#833924',
          900: '#3f1e15',
        },
      },
      fontFamily: {
        display: ['Fraunces', 'Georgia', 'serif'],
        sans: ['"Be Vietnam Pro"', 'system-ui', 'sans-serif'],
      },
      letterSpacing: {
        eyebrow: '0.22em',
      },
      boxShadow: {
        soft: '0 1px 2px rgba(42,33,27,.04), 0 8px 30px -12px rgba(42,33,27,.18)',
        lift: '0 24px 60px -28px rgba(42,33,27,.45)',
      },
      borderRadius: {
        '4xl': '2rem',
      },
      keyframes: {
        'fade-up': {
          '0%': { opacity: '0', transform: 'translateY(16px)' },
          '100%': { opacity: '1', transform: 'translateY(0)' },
        },
        'fade-in': {
          '0%': { opacity: '0' },
          '100%': { opacity: '1' },
        },
      },
      animation: {
        'fade-up': 'fade-up .7s cubic-bezier(.22,1,.36,1) both',
        'fade-in': 'fade-in .9s ease both',
      },
    },
  },
  plugins: [],
}
