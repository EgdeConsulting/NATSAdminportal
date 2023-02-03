import { Button, useColorMode } from "@chakra-ui/react";

function ColorModeButton() {
  const { colorMode, toggleColorMode } = useColorMode();
  return (
    <Button margin={2} size={"md"} onClick={() => toggleColorMode()}>
      {colorMode == "light" ? "🌙" : "☀️"}
    </Button>
  );
}

export { ColorModeButton };
