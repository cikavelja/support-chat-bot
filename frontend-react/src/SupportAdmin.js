import React, { useState } from "react";

const SupportAdminPage = () => {
  const [title, setTitle] = useState("");
  const [description, setDescription] = useState("");
  const [message, setMessage] = useState("");

  const addSupportTopic = async () => {
    if (!title.trim() || !description.trim()) {
      setMessage("❌ Please enter both title and description.");
      return;
    }

    const payload = { title, description };

    try {
      const response = await fetch("http://localhost:5140/api/SupportAdmin/add", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(payload),
      });

      if (!response.ok) {
        throw new Error(`Server error: ${response.status}`);
      }

      const data = await response.json(); // Expecting `{ message: "Success", id: 123 }`
      setMessage(`✅ ${data.message} (ID: ${data.id})`);
      setTitle("");
      setDescription("");
    } catch (error) {
      console.error("Error:", error);
      setMessage("❌ Error adding support topic.");
    }
  };

  return (
    <div className="container mt-4">
      <h2>Add Support Topic</h2>
      <input
        type="text"
        className="form-control my-2"
        placeholder="Title"
        value={title}
        onChange={(e) => setTitle(e.target.value)}
      />
      <textarea
        className="form-control my-2"
        placeholder="Description"
        value={description}
        onChange={(e) => setDescription(e.target.value)}
      />
      <button className="btn btn-primary" onClick={addSupportTopic}>
        Submit
      </button>
      {message && <p className="mt-3">{message}</p>}
    </div>
  );
};

export default SupportAdminPage;
