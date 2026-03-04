import { BrowserRouter, Routes, Route } from "react-router-dom";
import Layout from "./components/Layout";
import Home from "./pages/Home";
import BracketPage from "./pages/BracketPage";
import SimulatePage from "./pages/SimulatePage";

export default function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route element={<Layout />}>
          <Route index element={<Home />} />
          <Route path="bracket" element={<BracketPage />} />
          <Route path="simulate" element={<SimulatePage />} />
        </Route>
      </Routes>
    </BrowserRouter>
  );
}
