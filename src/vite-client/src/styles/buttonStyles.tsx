import { defineStyle, defineStyleConfig } from "@chakra-ui/styled-system";

const baseStyle = defineStyle({
  marginTop: -1,
});

const darkerBackground = defineStyle({
  bg: "gray.200",
  _dark: {
    bg: "whiteAlpha.200",
  },
  _hover: {
    bg: "gray.300",
    _dark: {
      bg: "whiteAlpha.300",
    },
  },
});

export const buttonStyles = defineStyleConfig({
  baseStyle,
  variants: {
    darkerBackground: darkerBackground,
  },
});
