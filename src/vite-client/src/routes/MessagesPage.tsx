import {
  Card,
  CardBody,
  HStack,
  VStack,
  Flex,
  Spacer,
  Box,
} from "@chakra-ui/react";
import { useEffect, useState } from "react";
import StickyBox from "react-sticky-box";
import {
  IMsg,
  MsgView,
  MsgTable,
  SubjectSidebar,
  MsgPublishModal,
  MsgContextProvider,
  MsgViewContextProvider,
  PageHeader,
} from "components";

function MessagesPage() {
  const [allMessages, setAllMessages] = useState<IMsg[]>([]);
  const [isIntervalRunning, setIsIntervalRunning] = useState(false);

  function getAllMessages() {
    fetch("/api/allMessages")
      .then((res) => res.json())
      .then((data: IMsg[]) => {
        setAllMessages(data);
      });
  }

  useEffect(() => {
    setIsIntervalRunning(true);
    const interval = setInterval(getAllMessages, 9000);
    return () => {
      clearInterval(interval);
      setIsIntervalRunning(false);
    };
  }, [!isIntervalRunning]);

  return (
    <MsgContextProvider>
      <MsgViewContextProvider>
        <HStack w={"100%"} align={"stretch"} pt={2}>
          <Flex w={"100%"}>
            <Card variant={"outline"} w={"70%"} mr={-2}>
              <CardBody>
                <HStack>
                  <PageHeader
                    heading={"All messages"}
                    introduction={
                      "This page shows all messages on all streams on the NATS-server."
                    }
                  />
                  <Spacer />
                  <Box pr={2}>
                    <MsgPublishModal />
                  </Box>
                </HStack>
                <MsgTable messages={allMessages} />
              </CardBody>
            </Card>
            <Spacer />
            <VStack w={"30%"} h={"100%"} mr={2}>
              <Box w={"95%"} h={"100%"} ml={4}>
                <StickyBox offsetTop={10}>
                  <MsgView />
                  <SubjectSidebar />
                </StickyBox>
              </Box>
            </VStack>
          </Flex>
        </HStack>
      </MsgViewContextProvider>
    </MsgContextProvider>
  );
}

export { MessagesPage };
