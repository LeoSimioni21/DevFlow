const isLocalhost = typeof window !== 'undefined' && window.location.hostname === 'localhost';

export const API_BASE_URL = isLocalhost
  ? 'http://localhost:5080/api'
  : 'https://devflow-itvz.onrender.com/api';
