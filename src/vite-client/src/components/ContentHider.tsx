import { Box, Button, Container, Text } from "@chakra-ui/react";
import { useState } from "react";

function ContentHider(props: { content: string }) {
  const [contentVisibility, setContentVisibility] = useState(false);
  return (
    <Box maxWidth={"900px"}>
      {contentVisibility ? (
        <Text>{props.content}</Text>
      ) : (
        <Button onClick={() => setContentVisibility(true)}>
          👁 Show Content
        </Button>
      )}
    </Box>
  );
}

export { ContentHider };
