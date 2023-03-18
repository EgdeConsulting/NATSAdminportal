import { Flex, Spacer, useColorMode, Box, HStack } from "@chakra-ui/react";
import { ColorModeButton, AccountMenu } from "components";
import { EgdeLogo, NatsLetterIcon } from "assets";
import { AddIcon } from "@chakra-ui/icons";

function Topbar() {
  const { colorMode, toggleColorMode } = useColorMode();
  return (
    <Flex
      maxH={"100px"}
      borderBottom={"1px solid"}
      borderColor={colorMode === "dark" ? "whiteAlpha.300" : "gray.200"}
    >
      <HStack ml={3} w={"300px"}>
        <NatsLetterIcon />
        <Box>
          <AddIcon mt={-3} />
        </Box>
        <EgdeLogo />
      </HStack>
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
