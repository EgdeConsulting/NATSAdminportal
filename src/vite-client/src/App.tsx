import {
  ChakraProvider,
  ColorModeProvider,
  CSSReset,
  HStack,
} from "@chakra-ui/react";

import { Topbar, Sidebar } from "./components";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import { SettingsPage, LogPage, HomePage } from "./routes";
import { theme } from "./styles";

function App() {
  return (
    <Router>
      <ChakraProvider theme={theme}>
        <ColorModeProvider>
          <CSSReset />
          <Topbar />
          <HStack align="stretch">
            <Sidebar />
            <Routes>
              <Route path="/log" element={<LogPage />} />
              <Route path="/settings" element={<SettingsPage />} />
              <Route path="/" element={<HomePage />} />
            </Routes>
          </HStack>
        </ColorModeProvider>
      </ChakraProvider>
    </Router>
  );
}

export { App };
