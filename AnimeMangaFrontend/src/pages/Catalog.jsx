import { useEffect, useState, useContext } from "react";
import api from "../api/axios";
import { AuthContext } from "../context/AuthContext";

export default function Catalog() {
  const { token } = useContext(AuthContext);
  const [entries, setEntries] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [message, setMessage] = useState("");

  useEffect(() => {
    const fetchCatalog = async () => {
      try {
        setLoading(true);
        const res = await api.get("/animemanga");
        setEntries(res.data);
      } catch (err) {
        console.error("Error fetching catalog:", err);
        setError("Failed to load catalog.");
      } finally {
        setLoading(false);
      }
    };

    fetchCatalog();
  }, []);

  const addToList = async (title) => {
    try {
      if (!token) {
        setMessage("You must be logged in to add items.");
        return;
      }
      await api.post("/userlists", { title, status: "Plan to Watch" });
      setMessage(`Added "${title}" to your list!`);
    } catch (err) {
      console.error("Error adding to list:", err);
      setMessage("Could not add item to your list.");
    }
  };

  if (loading) return <p>Loading catalog...</p>;
  if (error) return <p>{error}</p>;

  return (
    <div>
      <h1>Catalog</h1>
      {message && <p>{message}</p>}
      <ul>
        {entries.map((entry) => (
          <li key={entry.id}>
            <strong>{entry.title}</strong> ({entry.type}, {entry.year})
            {token && (
              <button onClick={() => addToList(entry.title)}>Add to My List</button>
            )}
          </li>
        ))}
      </ul>
    </div>
  );
}
