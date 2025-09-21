import { useEffect, useState, useContext } from "react";
import api from "../api/axios";
import { AuthContext } from "../context/AuthContext";

export default function MyList() {
  const { token } = useContext(AuthContext);
  const [list, setList] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    if (!token) return;

    const fetchList = async () => {
      try {
        setLoading(true);
        const res = await api.get("/userlists");
        setList(res.data);
      } catch (err) {
        console.error("Error fetching list:", err);
        setError("Failed to load list.");
      } finally {
        setLoading(false);
      }
    };

    fetchList();
  }, [token]);

  if (!token) {
    return <p>You must be logged in to view your list.</p>;
  }

  if (loading) return <p>Loading your list...</p>;
  if (error) return <p>{error}</p>;

  return (
    <div>
      <h1>My List</h1>
      {list.length === 0 ? (
        <p>Your list is empty.</p>
      ) : (
        <ul>
          {list.map((item) => (
            <li key={item.id}>
              <strong>{item.title}</strong>  
              — Status: {item.status}, Progress: {item.progress}, Notes: {item.notes}
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}
