import {
  Button,
  Card,
  CardHeader,
  Heading,
  Box,
  Stack,
  Flex,
  Text,
  Spacer,
  HStack,
  VStack,
} from "@chakra-ui/react";
import { useContext, useEffect, useState } from "react";
import {
  MsgDeleteModal,
  MsgContext,
  MsgViewContext,
  MsgCopyModal,
} from "components";
import { CloseIcon } from "@chakra-ui/icons";

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
      fetch("/api/specificMessage?" + queryString)
        .then((res) => res.json())
        .then((rawData) => {
          // JSON-server returns a JSON-array, whilest .NET-api returns a single JSON-object.
          let data = rawData instanceof Array ? rawData[0] : rawData;
          setMessageData(data);
        });
    }
  }

  useEffect(() => {
    getMessageData();
  }, [currentMsgContext]);

  return (
    <Stack w={"100%"} spacing={4}>
      {viewContext.isVisible && (
        <Card variant={"outline"} h={"100%"} w={"100%"} mb={2}>
          <CardHeader>
            <Flex>
              <Heading size={"md"}>Message Details</Heading>
              <Spacer />
              <HStack mt={-2} mr={-2}>
                <MsgCopyModal />
                <MsgDeleteModal />
                <Button
                  variant="ghost"
                  w={"25px"}
                  onClick={() => {
                    changeVisibility(false);
                  }}
                >
                  <CloseIcon />
                </Button>
              </HStack>
            </Flex>
          </CardHeader>
          <VStack ml={5} mb={5} align={"flex-start"} spacing={6}>
            <Box>
              <Heading size={"sm"} mb={2}>
                Headers
              </Heading>
              {messageData &&
              messageData.headers != undefined &&
              Object.entries(messageData.headers).length != 0 ? (
                messageData.headers.map((headerPair: any, index: number) => (
                  <Text key={index} fontSize={"md"}>
                    {headerPair.name + " : " + headerPair.value}
                  </Text>
                ))
              ) : (
                <Text fontSize={"md"}>No Headers...</Text>
              )}
            </Box>
            <Box>
              <Heading size={"sm"} mb={2}>
                Payload
              </Heading>
              <Text fontSize={"md"}>{messageData.payload}</Text>
            </Box>
          </VStack>
        </Card>
      )}
    </Stack>
  );
}

export { MsgView };
