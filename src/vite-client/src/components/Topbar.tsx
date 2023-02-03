import {
  Image,
  Button,
  Flex,
  Spacer,
  useColorMode,
  Box,
} from "@chakra-ui/react";
import { ColorModeButton, AccountMenu } from "./";
import natslogo from "../assets/nats-letter-icon.svg";

function Topbar() {
  const { colorMode, toggleColorMode } = useColorMode();
  return (
    <div>
      <Flex maxH={"100px"} border={"thin"}>
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
      <hr />
    </div>
  );
}

export { Topbar };
