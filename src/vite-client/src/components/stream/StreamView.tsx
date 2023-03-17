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
import { StreamContext, StreamViewContext } from "components";
import { CloseIcon } from "@chakra-ui/icons";

function StreamView() {
  const currentStreamContext = useContext(StreamContext);
  const { changeVisibility } = useContext(StreamViewContext);
  const viewContext = useContext(StreamViewContext);
  const [streamData, setStreamData] = useState<any>([]);

  function getStreamData() {
    const stream = currentStreamContext.currentStream;
    if (stream.name) {
      const queryString = "streamName=" + stream.name;
      fetch("/api/streamData?" + queryString)
        .then((res) => res.json())
        .then((data) => {
          setStreamData(data);
        });
    }
  }

  useEffect(() => {
    getStreamData();
  }, [currentStreamContext]);

  return (
    <Stack w={"100%"} spacing={4}>
      {viewContext.isVisible && (
        <Card variant={"outline"} h={"100%"} w={"100%"} mb={2}>
          <CardHeader>
            <Flex>
              <Heading size={"md"}>
                Details for stream: "{streamData.name}"
              </Heading>
              <Spacer />
              <HStack mt={-2} mr={-2}>
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
          <VStack
            ml={5}
            mb={5}
            align={"flex-start"}
            spacing={6}
            divider={<StackDivider />}
          >
            <Box>
              <Heading size={"sm"} mb={2}>
                Description
              </Heading>
              {streamData.description != undefined &&
              streamData.description.length != 0 ? (
                streamData.desciption
              ) : (
                <Text>No description...</Text>
              )}
            </Box>
            <Box>
              <Heading size={"sm"} mb={2}>
                Subjects
              </Heading>
              {streamData.subjects != undefined &&
                streamData.subjects.map((subject: string, key: number) => {
                  return <Text key={key}>{subject}</Text>;
                })}
            </Box>
            <Box>
              <Heading size={"sm"} mb={2}>
                Consumers
              </Heading>
              {streamData.consumers != undefined &&
              streamData.consumers.length != 0 ? (
                streamData.consumers.map((consumer: string, key: number) => {
                  return <Text key={key}>{consumer}</Text>;
                })
              ) : (
                <Text>No consumers...</Text>
              )}
            </Box>
            <Box>
              <Heading size={"sm"} mb={2}>
                Policies
              </Heading>
              {streamData.policies != undefined &&
              Object.entries(streamData.policies).length != 0 ? (
                Object.entries(streamData.policies).map(
                  ([key, value], index: number) => (
                    <Text key={index} fontSize={"md"}>
                      {key + " : " + value}
                    </Text>
                  )
                )
              ) : (
                <Text fontSize={"md"}>No Policies...</Text>
              )}
            </Box>
            <Box>
              <Heading size={"sm"} mb={2}>
                Deleted
              </Heading>
              {
                <Text fontSize={"md"}>
                  Deleted messages:{" "}
                  {streamData.deleted != undefined && streamData.deleted}
                </Text>
              }
            </Box>
          </VStack>
        </Card>
      )}
    </Stack>
  );
}

export { StreamView };
