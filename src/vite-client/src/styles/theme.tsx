import { extendTheme } from "@chakra-ui/react";
import { inputStyles } from "styles";
import { buttonStyles } from "styles";
import { cardStyles } from "styles";
import { textStyles } from "styles";

const theme = extendTheme({
  components: {
    Button: buttonStyles,
    Card: cardStyles,
    Text: textStyles,
    Input: inputStyles,
  },
});

export { theme };
