import {
  Button,
  Card,
  CardHeader,
  Heading,
  Box,
  Stack,
  Flex,
  StackDivider,
  Text,
  Spacer,
  HStack,
  VStack,
} from "@chakra-ui/react";
import { useContext, useEffect, useState } from "react";
import { MsgDeleteModal, MsgContext, MsgViewContext } from "components";
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
      fetch("/api/messageData?" + queryString)
        .then((res) => res.json())
        .then((data) => {
          setMessageData(data);
        });
    }
  }

  useEffect(() => {
    getMessageData();
  }, [currentMsgContext]);

  return (
    <Stack w={"100%"} spacing={4}>
      {viewContext.isVisiable && (
        <Card variant={"outline"} h={"100%"} w={"100%"}>
          <CardHeader>
            <Flex>
              <Heading size={"md"}>Message Details</Heading>
              <Spacer />
              <HStack mt={-15} mr={-2}>
                <MsgDeleteModal />
                <Button
                  variant="ghost"
                  w={"25px"}
                  mb={2}
                  mr={3}
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
              <Heading size={"sm"} marginBottom={2}>
                Headers
              </Heading>
              {messageData &&
              messageData.headers != undefined &&
              Object.entries(messageData.headers).length != 0 ? (
                messageData.headers.map((headerPair: any, index: number) => (
                  //Snakk me gunther imåro...
                  <Text key={index} fontSize={"md"}>
                    {headerPair.name + " : " + headerPair.value}
                  </Text>
                ))
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
          </VStack>
        </Card>
      )}
    </Stack>
  );
}

export { MsgView };
