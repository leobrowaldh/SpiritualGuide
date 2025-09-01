import { Route, Routes } from "react-router";
import AskPage from "../pages/ask";
import Home from "../pages/home";

export function AppRouter(){
    return (
        <Routes>
            <Route path="/" element={<Home/>} />
            <Route path="/ask" element={<AskPage/>} />
        </Routes>
    )
}