import { StyleFunctionProps } from "@chakra-ui/react";
import { defineStyle, defineStyleConfig } from "@chakra-ui/styled-system";

const baseStyle = defineStyle({
  marginTop: -1,
});

const darkerBackground = defineStyle((props: StyleFunctionProps) => ({
  bg: props.colorMode === "light" ? "gray.200" : "whiteAlpha.200",
  _hover: {
    bg: props.colorMode === "light" ? "gray.300" : "whiteAlpha.300",
  },
}));

export const buttonStyles = defineStyleConfig({
  baseStyle,
  variants: {
    darkerBackground: darkerBackground,
  },
});
