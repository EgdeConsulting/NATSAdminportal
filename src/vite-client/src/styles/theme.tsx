import { extendTheme } from "@chakra-ui/react";
import { inputStyles } from "./inputStyles";
import { buttonStyles } from "./buttonStyles";
import { cardStyles } from "./cardStyles";
import { textStyles } from "./textStyles";

const theme = extendTheme({
  components: {
    Button: buttonStyles,
    Card: cardStyles,
    Text: textStyles,
    Input: inputStyles,
  },
});

export { theme };
