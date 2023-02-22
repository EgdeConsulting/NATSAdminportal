import { Image, Flex, Spacer, useColorMode, Box } from "@chakra-ui/react";
import { ColorModeButton, AccountMenu } from "components";
import natslogo from "../assets/nats-letter-icon.svg";

function Topbar() {
  const { colorMode, toggleColorMode } = useColorMode();
  return (
    <Flex
      maxH={"100px"}
      borderBottom={"1px solid"}
      borderColor={colorMode === "dark" ? "whiteAlpha.300" : "gray.200"}
    >
      <Box marginLeft={3}>
        <Image src={natslogo} alt={"NATS Logo"} />
      </Box>
      <Spacer />
      <Box>
        <ColorModeButton />
      </Box>
      <Box>
        <AccountMenu />
      </Box>
    </Flex>
  );
}

export { Topbar };
