import { extendTheme, type ThemeConfig } from "@chakra-ui/react";
import {
  InputStyles,
  ButtonStyles,
  CardStyles,
  TextStyles,
  AccordionStyles,
} from "styles";

const config: ThemeConfig = {
  initialColorMode: "dark",
  useSystemColorMode: true,
};

const Theme = extendTheme({
  config,
  colors: {
    gray: {
      "50": "#F2F2F3",
      "100": "#DADBDD",
      "200": "#C2C4C7",
      "300": "#AAADB1",
    },
  },
  components: {
    Button: ButtonStyles,
    Card: CardStyles,
    Text: TextStyles,
    Input: InputStyles,
    Accordion: AccordionStyles,
  },
});

export { Theme };
