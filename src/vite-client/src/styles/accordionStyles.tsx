import { accordionAnatomy } from "@chakra-ui/anatomy";
import { createMultiStyleConfigHelpers } from "@chakra-ui/react";

const { definePartsStyle, defineMultiStyleConfig } =
  createMultiStyleConfigHelpers(accordionAnatomy.keys);

const baseStyle = definePartsStyle({
  // define the part you're going to style
  container: {
    borderTopWidth: "0px",
    fontSize: "lg",
    _last: {
      borderBottomWidth: "0px",
    },
  },
  button: {
    fontSize: "xl",
  },
});

export const accordionStyles = defineMultiStyleConfig({ baseStyle });
