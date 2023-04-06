import { extendTheme } from "@chakra-ui/react";
import {
  inputStyles,
  buttonStyles,
  cardStyles,
  textStyles,
  accordionStyles,
} from "styles";

const theme = extendTheme({
  components: {
    Button: buttonStyles,
    Card: cardStyles,
    Text: textStyles,
    Input: inputStyles,
    Accordion: accordionStyles,
  },
});

export { theme };
