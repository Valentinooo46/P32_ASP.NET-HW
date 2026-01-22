const API_BASE_URL = (typeof import.meta !== "undefined" ? import.meta.env?.VITE_API_BASE_URL : undefined) || "http://localhost:5055";

const APP_ENV = {
  API_BASE_URL,
};

export default APP_ENV;
