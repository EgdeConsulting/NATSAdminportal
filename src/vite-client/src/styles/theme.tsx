import { extendTheme, type ThemeConfig } from "@chakra-ui/react";
import { inputStyles, buttonStyles, cardStyles, textStyles } from "styles";

const config: ThemeConfig = {
  initialColorMode: "dark",
  useSystemColorMode: true,
};

const theme = extendTheme({
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
    Button: buttonStyles,
    Card: cardStyles,
    Text: textStyles,
    Input: inputStyles,
  },
});

export { theme };
