import { defineStyle, defineStyleConfig } from "@chakra-ui/styled-system";

const baseStyle = defineStyle({
  fontSize: "lg",
});

export const textStyles = defineStyleConfig({
  baseStyle,
});
