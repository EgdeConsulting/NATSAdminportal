import {
  ChakraProvider,
  ColorModeProvider,
  CSSReset,
  HStack,
  Box,
} from "@chakra-ui/react";

import { Topbar, Sidebar } from "./components";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import { MessagesPage, HomePage, StreamsPage } from "./routes";
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
            <Box w={"100%"} h={"100%"} pb={2}>
              <Routes>
                <Route path="/streams" element={<StreamsPage />} />
                <Route path="/messages" element={<MessagesPage />} />
                <Route path="/" element={<HomePage />} />
                <Route path="/*" element={<HomePage />} />
              </Routes>
            </Box>
          </HStack>
        </ColorModeProvider>
      </ChakraProvider>
    </Router>
  );
}

export { App };
