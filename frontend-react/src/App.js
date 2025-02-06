import './App.css';
import Chat from './Chat';
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import SupportAdminPage from './SupportAdmin'; // Import the new view

function App() {
  return (
    <Router>
    <Routes>
      <Route path="/" element={<Chat />} />  {/* Your main home page */}
      <Route path="/support-admin" element={<SupportAdminPage />} /> {/* New view */}
    </Routes>
  </Router>
  );
}

export default App;
