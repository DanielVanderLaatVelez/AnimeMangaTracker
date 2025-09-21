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

  const updateItem = async (id, updates) => {
  try {
    const res = await api.put(`/userlists/${id}`, updates);
    // Update state with new values
    setList((prev) =>
      prev.map((item) => (item.id === id ? res.data : item))
    );
  } catch (err) {
    console.error("Error updating item:", err);
  }
};

const deleteItem = async (id) => {
  try {
    await api.delete(`/userlists/${id}`);
    // Remove from local state
    setList((prev) => prev.filter((item) => item.id !== id));
  } catch (err) {
    console.error("Error deleting item:", err);
  }
};


  return (
    <div>
      <h1>My List</h1>
      {list.length === 0 ? (
        <p>Your list is empty.</p>
      ) : (
        <ul>
          {list.map((item) => (
            <li key={item.id}>
              <strong>{item.title}</strong> — Status: {item.status}, Progress: {item.progress}, Notes: {item.notes}
              <div>
                {/* Update Status */}
                <select
                  value={item.status}
                  onChange={(e) =>
                    updateItem(item.id, { ...item, status: e.target.value })
                  }
                >
                  <option>Watching</option>
                  <option>Completed</option>
                  <option>On-Hold</option>
                  <option>Dropped</option>
                  <option>Plan to Watch</option>
                </select>

                {/* Update Progress */}
                <input
                  type="number"
                  min="0"
                  value={item.progress}
                  onChange={(e) =>
                    updateItem(item.id, { ...item, progress: Number(e.target.value) })
                  }
                />

                {/* Update Notes */}
                <input
                  type="text"
                  value={item.notes}
                  onChange={(e) =>
                    updateItem(item.id, { ...item, notes: e.target.value })
                  }
                />


                {/* Delete */}
                <button onClick={() => deleteItem(item.id)}>Remove</button>
              </div>
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}
