import { useState } from "react";
import { useNavigate } from "react-router-dom";
import api from "../api/axios";
import "../App.css";

export default function Login() {
  const navigate = useNavigate();

  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [errors, setErrors] = useState<any>({});

  const validate = () => {
    let err: any = {};
    if (!username) err.username = "Username is required";
    if (!password) err.password = "Password is required";
    return err;
  };

  const handleLogin = async () => {
    const validationErrors = validate();

    if (Object.keys(validationErrors).length > 0) {
      setErrors(validationErrors);
      return;
    }

    try {
      setErrors({});

      const res = await api.post("/auth/login", { username, password });

      localStorage.setItem("token", res.data.data.token);
      localStorage.setItem("userId", res.data.data.userId);

      navigate("/leads"); 
      

    } catch (err: any) {
      const message =
        err?.response?.data?.message ||
        err?.response?.data?.errors?.[0] ||
        "Invalid username or password";

      setErrors({ api: message });
    }
  };

  return (
    <div className="login-wrapper">
      <form
        className="login-card"
        onSubmit={(e) => {
          e.preventDefault();
          handleLogin();
        }}
      >
        <h2 className="login-title">Sign in</h2>

        {errors.api && <div className="login-error">{errors.api}</div>}

        <div className="login-field">
          <label>Username</label>
          <input
            value={username}
            className={errors.username ? "input-error" : ""}
            onChange={(e) => {
              setUsername(e.target.value);
              setErrors({});
            }}
          />
          {errors.username && <span>{errors.username}</span>}
        </div>

        <div className="login-field">
          <label>Password</label>
          <input
            type="password"
            value={password}
            className={errors.password ? "input-error" : ""}
            onChange={(e) => {
              setPassword(e.target.value);
              setErrors({});
            }}
          />
          {errors.password && <span>{errors.password}</span>}
        </div>

        <button className="login-btn" type="submit">Sign in</button>
      </form>
    </div>
  );
}