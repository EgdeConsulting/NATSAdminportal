import { Box, Button, Text } from "@chakra-ui/react";
import { useTracking } from "react-tracking";
import { useState } from "react";

// https://codesandbox.io/s/github/tanstack/table/tree/main/examples/react/pagination?from-embed=&file=/src/main.tsx:6330-6393
// JSON.stringify
//

function ContentHider(props: { trackingData: string; content: string }) {
  const [contentVisibility, setContentVisibility] = useState(false);
  const { trackEvent } = useTracking();

  return (
    <Box maxWidth={"900px"}>
      {contentVisibility ? (
        <Text>{props.content}</Text>
      ) : (
        <Button
          onClick={() => {
            trackEvent({
              account: window.accountLayer,
              event: "show-row-content",
              origin: props.trackingData,
            });
            setContentVisibility(true);
          }}
        >
          👁 Show Content
        </Button>
      )}
    </Box>
  );
}

export { ContentHider };
