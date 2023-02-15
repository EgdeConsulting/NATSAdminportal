import { IconButton, Box, VStack, useColorMode } from "@chakra-ui/react";
import { FiHome, FiSettings, FiArchive } from "react-icons/fi";
import { HamburgerIcon } from "@chakra-ui/icons";
import { useState } from "react";
import { NavItem } from "./NavItem";

function Sidebar() {
  const { colorMode, toggleColorMode } = useColorMode();
  const [navSize, changeNavSize] = useState(200);
  const [size, setSize] = useState(50);

  function togglePanel() {
    setSize(size == 135 ? 50 : 135);
    changeNavSize(navSize == 50 ? 200 : 50);
  }

  return (
    <VStack
      h="100vh"
      w={size + "px"}
      style={{ minWidth: size + "px", maxWidth: size + "px" }}
      borderRight={"1px solid"}
      borderColor={colorMode === "dark" ? "whiteAlpha.300" : "gray.200"}
    >
      <Box w="100%" h="40px" p="2">
        <IconButton
          _hover={{
            textDecor: "none",
            backgroundColor:
              colorMode === "dark" ? "whiteAlpha.300" : "gray.200",
          }}
          bg="none"
          borderRadius={8}
          aria-label="Panel button"
          onClick={() => {
            togglePanel();
          }}
          size="sm"
          icon={<HamburgerIcon boxSize={6} />}
        ></IconButton>
      </Box>

      <Box>
        <VStack>
          <NavItem
            navSize={navSize}
            icon={FiHome}
            title="Home"
            route="/"
            width={size - 25}
          />
          <NavItem
            navSize={navSize}
            icon={FiSettings}
            title="Settings"
            route="/settings"
            width={size - 25}
          />
          <NavItem
            navSize={navSize}
            icon={FiArchive}
            title="Log"
            route="/log"
            width={size - 25}
          />
        </VStack>
      </Box>
    </VStack>
  );
}

export { Sidebar };
