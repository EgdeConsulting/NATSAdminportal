import {
  ChakraProvider,
  Box,
  Flex,
  Card,
  Input,
  ColorModeProvider,
  CSSReset,
  HStack,
} from "@chakra-ui/react";

import { Topbar, SideBar } from "./components";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import { Settings, Log, Home } from "./routes";
import { theme } from "./styles";


function App() {
  return (
    <Router>
      <ChakraProvider theme={theme}>
        <ColorModeProvider>
          <CSSReset />
          <Topbar />
          <HStack align="stretch">
            <SideBar />
            <Routes>
              <Route path="/log" element={<Log />} />
              <Route path="/settings" element={<Settings />} />
              <Route path="/" element={<Home />} />
            </Routes>
          </HStack>
        </ColorModeProvider>
      </ChakraProvider>
    </Router>
  );
}

export { App };
