import {
  Button,
  Card,
  CardHeader,
  Heading,
  Box,
  Stack,
  StackDivider,
  Text,
} from "@chakra-ui/react";
import { useContext, useEffect, useState } from "react";
import { MsgDeleteModal, MsgContext, MsgViewContext } from "components";

function MsgView() {
  const [messageData, setMessageData] = useState<any>([]);
  const currentMsgContext = useContext(MsgContext);
  const { changeVisibility } = useContext(MsgViewContext);
  const viewContext = useContext(MsgViewContext);

  function getMessageData() {
    const msg = currentMsgContext.currentMsg;
    if (msg.stream && msg.sequenceNumber) {
      const queryString =
        "streamName=" + msg.stream + "&sequenceNumber=" + msg.sequenceNumber;
      fetch("/api/messageData?" + queryString)
        .then((res) => res.json())
        .then((data) => {
          setMessageData(data);
        });
    }
  }

  useEffect(() => {
    getMessageData();
  });

  return (
    <Stack divider={<StackDivider />} spacing={4}>
      {viewContext.isVisiable && (
        <Card variant={"outline"} h={"100%"} w={"100%"}>
          <CardHeader>
            <Heading size={"md"}>Message Details</Heading>
          </CardHeader>
          <Box>
            <Heading size={"sm"} marginBottom={2}>
              Headers
            </Heading>
            {messageData &&
            messageData.headers != undefined &&
            Object.entries(messageData.headers).length != 0 ? (
              Object.entries(messageData.headers).map(
                ([key, value], index: number) => (
                  <Text key={index} fontSize={"md"}>
                    {key + " : " + value}
                  </Text>
                )
              )
            ) : (
              <Text fontSize={"md"}>No Headers...</Text>
            )}
          </Box>
          <Box>
            <Heading size={"sm"} marginBottom={2}>
              Payload
            </Heading>
            <Text fontSize={"md"}>{messageData.payload}</Text>
          </Box>
          <MsgDeleteModal />
          <Button
            variant="ghost"
            mb={2}
            mr={3}
            onClick={() => {
              changeVisibility(false);
            }}
          >
            Close
          </Button>
        </Card>
      )}
    </Stack>
  );
}

export { MsgView };
