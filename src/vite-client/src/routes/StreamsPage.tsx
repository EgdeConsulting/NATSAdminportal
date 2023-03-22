import {
  Box,
  Card,
  CardBody,
  Flex,
  HStack,
  Spacer,
  VStack,
} from "@chakra-ui/react";
import { useEffect, useState } from "react";
import StickyBox from "react-sticky-box";
import {
  StreamView,
  StreamTable,
  StreamContextProvider,
  StreamViewContextProvider,
  PageHeader,
} from "components";

function StreamsPage() {
  const [streams, setStreams] = useState<any[]>([]);

  useEffect(() => {
    getStreams();
  }, [streams.length != 0]);

  function getStreams() {
    fetch("/api/allStreams")
      .then((res) => res.json())
      .then((data) => {
        setStreams(data);
      });
  }

  return (
    <StreamContextProvider>
      <StreamViewContextProvider>
        <HStack w={"100%"} align={"stretch"} pt={2}>
          <Flex w={"100%"}>
            <Card variant={"outline"} w={"70%"} mr={-2}>
              <CardBody>
                <PageHeader
                  heading={"All streams"}
                  introduction={
                    "This page shows all streams on the NATS-server."
                  }
                />
                <StreamTable streamInfo={streams} />
              </CardBody>
            </Card>
            <Spacer />
            <VStack w={"30%"} h={"100%"} mr={2}>
              <Box w={"95%"} h={"100%"} ml={4}>
                <StickyBox offsetTop={10}>
                  <StreamView />
                </StickyBox>
              </Box>
            </VStack>
          </Flex>
        </HStack>
      </StreamViewContextProvider>
    </StreamContextProvider>
  );
}

export { StreamsPage };
