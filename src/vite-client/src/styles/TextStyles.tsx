import { defineStyle, defineStyleConfig } from "@chakra-ui/styled-system";

const baseStyle = defineStyle({
  fontSize: "lg",
});

const greyedOut = defineStyle({
  color: "gray.300",
  _dark: {
    color: "whiteAlpha.400",
  },
});

export const TextStyles = defineStyleConfig({
  baseStyle,
  variants: {
    greyedOut: greyedOut,
  },
});
