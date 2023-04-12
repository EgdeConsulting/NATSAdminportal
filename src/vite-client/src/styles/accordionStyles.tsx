import { accordionAnatomy } from "@chakra-ui/anatomy";
import { createMultiStyleConfigHelpers } from "@chakra-ui/react";
const { definePartsStyle, defineMultiStyleConfig } =
  createMultiStyleConfigHelpers(accordionAnatomy.keys);

const baseStyle = definePartsStyle({
  // define the part you're going to style
  container: {
    borderTopWidth: "0px",
    fontSize: "md",
    _last: {
      borderBottomWidth: "0px",
    },
  },
  button: {
    fontSize: "lg",
    _hover: {
      _dark: {
        bg: "whiteAlpha.200",
      },
    },
  },
});

export const accordionStyles = defineMultiStyleConfig({ baseStyle });
