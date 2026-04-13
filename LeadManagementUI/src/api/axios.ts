import axios from "axios";

const api = axios.create({
  baseURL: "https://localhost:44337/api/v1",
});

api.interceptors.request.use((config) => {
  const token = localStorage.getItem("token");
  if (token) config.headers.Authorization = `Bearer ${token}`;
  return config;
});

api.interceptors.response.use(
  (res) => res,
  (err) => {
    if (err.response?.status === 401) {
      localStorage.clear();
      window.location.href = "/";
      return Promise.reject(err);;
    }

    if (err.response?.status === 403) {
      window.location.href = "/unauthorized";
      return;
    }

    const message =
      err.response?.data?.message ||
      err.response?.data?.title ||
      "User Unauthorised to access the service";

    return Promise.reject({
      ...err,
      customMessage: message,
    });
  }
);

export default api;