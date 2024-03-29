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
  StackDivider,
} from "@chakra-ui/react";
import { useContext, useEffect, useState } from "react";
import {
  MsgDeleteModal,
  MsgContext,
  MsgViewContext,
  MsgCopyModal,
  MsgContextType,
  DefaultMsgState,
  SpecificMsgProps,
  HeaderProps,
  LoadingSpinner,
  MsgPayloadModal,
} from "components";
import { CloseIcon } from "@chakra-ui/icons";

function MsgView() {
  const [messageData, setMessageData] = useState<SpecificMsgProps>();
  const currentMsgContext = useContext(MsgContext);
  const { changeVisibility } = useContext(MsgViewContext);
  const { changeCurrentMsg } = useContext(MsgContext) as MsgContextType;
  const viewContext = useContext(MsgViewContext);
  const [loading, setLoading] = useState(false);

  function getMessageData() {
    setLoading(true);
    const msg = currentMsgContext.currentMsg;
    if (msg.stream && msg.sequenceNumber) {
      const queryString =
        "streamName=" + msg.stream + "&sequenceNumber=" + msg.sequenceNumber;
      fetch("/api/specificMessage?" + queryString)
        .then((res) => res.json())
        .then((rawData: SpecificMsgProps) => {
          // JSON-server returns a JSON-array, whilst the API returns a single JSON-object.
          let data = rawData instanceof Array ? rawData[0] : rawData;
          setMessageData(data);
          setLoading(false);
        });
    }
  }

  useEffect(() => {
    getMessageData();
  }, [currentMsgContext]);

  return (
    <Stack w={"100%"} spacing={4}>
      {viewContext.isVisible && (
        <Card
          overflowY={"auto"}
          h={"60vh"}
          variant={"outline"}
          p={"absolute"}
          w={"100%"}
          mb={2}
        >
          <VStack ml={5} divider={<StackDivider pr={3} w={"96%"} />}>
            <CardHeader w={"100%"} ml={-10}>
              <Flex>
                <Heading size={"md"}>Message Details</Heading>
                <Spacer />
                <HStack mt={-2} mr={-7}>
                  <MsgCopyModal />
                  <MsgDeleteModal />
                  <Button
                    variant="ghost"
                    w={"25px"}
                    onClick={() => {
                      changeVisibility(false);
                      changeCurrentMsg(DefaultMsgState.currentMsg);
                    }}
                    title={"Close Button"}
                  >
                    <CloseIcon />
                  </Button>
                </HStack>
              </Flex>
            </CardHeader>

            {loading ? (
              <LoadingSpinner />
            ) : (
              <VStack
                w={"100%"}
                mt={4}
                mb={5}
                align={"flex-start"}
                spacing={6}
                divider={<StackDivider w={"96%"} />}
              >
                <Box>
                  <Heading size={"sm"} mb={2}>
                    Stream
                  </Heading>
                  <Text fontSize={"md"}>
                    {currentMsgContext.currentMsg.stream}
                  </Text>
                </Box>
                <Box>
                  <Heading size={"sm"} mb={2}>
                    Sequence number
                  </Heading>
                  <Text fontSize={"md"}>
                    {currentMsgContext.currentMsg.sequenceNumber}
                  </Text>
                </Box>
                <Box>
                  <Heading size={"sm"} mb={2}>
                    Subject
                  </Heading>
                  <Text fontSize={"md"}>
                    {currentMsgContext.currentMsg.subject}
                  </Text>
                </Box>
                <Box>
                  <Heading size={"sm"} mb={2}>
                    Headers
                  </Heading>
                  {messageData &&
                  messageData.headers != undefined &&
                  Object.entries(messageData.headers).length != 0 ? (
                    messageData.headers.map(
                      (headerPair: HeaderProps, index: number) => (
                        <Text key={index} fontSize={"md"}>
                          {headerPair.name + " : " + headerPair.value}
                        </Text>
                      )
                    )
                  ) : (
                    <Text fontSize={"md"}>No Headers...</Text>
                  )}
                </Box>
                <Box>
                  <Heading size={"sm"} mb={2}>
                    Payload
                  </Heading>
                  <Text mb={3} fontSize={"md"}>
                    {messageData?.payload
                      ? messageData.payload.data
                      : "No Payload..."}
                  </Text>
                  <MsgPayloadModal />
                </Box>
              </VStack>
            )}
          </VStack>
        </Card>
      )}
    </Stack>
  );
}

export { MsgView };
