import { extendTheme } from "@chakra-ui/react";
import { inputTheme } from "./inputTheme";

const theme = extendTheme({
  components: {
    Button: {
      baseStyle: {
        marginTop: -1,
      },
    },
    Card: {
      defaultProps: {
        variant: "outline",
      },
    },
    Input: inputTheme,
  },
});

export { theme };
