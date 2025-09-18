import { Link } from "react-router-dom";

export default function Navbar() {
  return (
    <nav>
      <ul style={{ display: "flex", gap: "20px", listStyle: "none" }}>
        <li><Link to="/">Home</Link></li>
        <li><Link to="/login">Login</Link></li>
        <li><Link to="/register">Register</Link></li>
        <li><Link to="/catalog">Catalog</Link></li>
        <li><Link to="/mylist">My List</Link></li>
      </ul>
    </nav>
  );
}
