import { IconButton, Card, Box, VStack, useColorMode } from "@chakra-ui/react";
import { FiHome, FiMail, FiLayers } from "react-icons/fi";
import { HamburgerIcon } from "@chakra-ui/icons";
import { useState } from "react";
import { NavItem } from "components";
import StickyBox from "react-sticky-box";

function Sidebar() {
  const { colorMode } = useColorMode();
  const [navSize, changeNavSize] = useState(200);
  const [size, setSize] = useState(50);

  function togglePanel() {
    setSize(size == 150 ? 50 : 150);
    changeNavSize(navSize == 50 ? 200 : 50);
  }

  return (
    <VStack
      w={size + "px"}
      style={{ minWidth: size + "px", maxWidth: size + "px" }}
      borderRight={"1px solid"}
      borderColor={colorMode === "dark" ? "whiteAlpha.300" : "gray.200"}
    >
      <StickyBox offsetTop={10}>
        <Card h={"94vh"} border={"none"}>
          <Box h="40px" p="2">
            <IconButton
              width={"100%"}
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
            />
          </Box>

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
              icon={FiMail}
              title="Messages"
              route="/messages"
              width={size - 25}
            />

            <NavItem
              navSize={navSize}
              icon={FiLayers}
              title="Streams"
              route="/streams"
              width={size - 25}
            />
          </VStack>
        </Card>
      </StickyBox>
    </VStack>
  );
}

export { Sidebar };
