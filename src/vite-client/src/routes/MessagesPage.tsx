import {
  Card,
  CardBody,
  HStack,
  Button,
  VStack,
  Flex,
  Spacer,
  Box,
  Container,
  Spinner,
  Center,
} from "@chakra-ui/react";
import { useEffect, useState } from "react";
import StickyBox from "react-sticky-box";
import {
  MsgView,
  MsgTable,
  SubjectSidebar,
  MsgPublishModal,
  MsgContextProvider,
  MsgViewContextProvider,
  PageHeader,
  LoadingSpinner,
} from "components";

function MessagesPage() {
  const [allMessages, setAllMessages] = useState<any[]>([]);
  const [isIntervalRunning, setIsIntervalRunning] = useState(false);
  const [loading, setLoading] = useState(true);

  function getAllMessages() {
    fetch("/api/allMessages")
      .then((res: any) => res.json())
      .then((data) => {
        setAllMessages(data);
        setLoading(false);
      });
  }

  useEffect(() => {
    setIsIntervalRunning(true);
    const interval = setInterval(getAllMessages, 1000);
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
                {loading ? (
                  <LoadingSpinner spinnerHeight={"300px"} />
                ) : (
                  <MsgTable messages={allMessages} />
                )}
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
