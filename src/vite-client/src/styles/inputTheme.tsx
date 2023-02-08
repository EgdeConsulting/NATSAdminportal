import { inputAnatomy } from "@chakra-ui/anatomy";
import { createMultiStyleConfigHelpers } from "@chakra-ui/react";

const { definePartsStyle, defineMultiStyleConfig } =
  createMultiStyleConfigHelpers(inputAnatomy.keys);

const baseStyle = definePartsStyle({
  field: {
    width: 500,
    marginTop: 2,
    marginRight: 2,
    type: "text",
  },
});

export const inputTheme = defineMultiStyleConfig({ baseStyle });
