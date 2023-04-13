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
  StreamContext,
  StreamViewContext,
  LoadingSpinner,
  IStreamExtended,
} from "components";

import { CloseIcon } from "@chakra-ui/icons";

function StreamView() {
  const currentStreamContext = useContext(StreamContext);
  const { changeVisibility } = useContext(StreamViewContext);
  const viewContext = useContext(StreamViewContext);
  const [streamData, setStreamData] = useState<IStreamExtended>();
  const [loading, setLoading] = useState(false);

  function getStreamData() {
    setLoading(true);
    const stream = currentStreamContext.currentStream;
    if (stream.name) {
      const queryString = "streamName=" + stream.name;
      fetch("/api/specificStream?" + queryString)
        .then((res) => res.json())
        .then((rawData: IStreamExtended) => {
          // JSON-server returns a JSON-array, whilest .NET-api returns a single JSON-object.
          let data = rawData instanceof Array ? rawData[0] : rawData;
          setStreamData(data);
          setLoading(false);
        });
    }
  }

  useEffect(() => {
    getStreamData();
  }, [currentStreamContext]);

  return (
    <Stack w={"100%"} spacing={4}>
      {viewContext.isVisible && (
        <Card
          overflowY={"auto"}
          h={"92vh"}
          variant={"outline"}
          p={"absolute"}
          w={"100%"}
          mb={2}
        >
          <VStack ml={5} divider={<StackDivider ml={5} w={"93%"} />}>
            <CardHeader w={"100%"} ml={-10}>
              <Flex>
                <Heading size={"md"}>Stream Details</Heading>
                <Spacer />
                <HStack mt={-1} mr={-7}>
                  <Button
                    variant="ghost"
                    w={"25px"}
                    onClick={() => {
                      changeVisibility(false);
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
              streamData && (
                <VStack
                  w={"100%"}
                  mt={4}
                  mb={5}
                  align={"flex-start"}
                  spacing={6}
                  divider={<StackDivider w={"93%"} />}
                >
                  <Box>
                    <Heading size={"sm"} mb={2}>
                      Name
                    </Heading>
                    <Text>{currentStreamContext.currentStream.name}</Text>
                  </Box>
                  <Box>
                    <Heading size={"sm"} mb={2}>
                      Description
                    </Heading>
                    {streamData.description != undefined &&
                    streamData.description.length != 0 ? (
                      streamData.description
                    ) : (
                      <Text>No description...</Text>
                    )}
                  </Box>
                  <Box>
                    <Heading size={"sm"} mb={2}>
                      Subjects
                    </Heading>
                    {streamData.subjects != undefined &&
                      streamData.subjects.map(
                        (subject: string, key: number) => {
                          return <Text key={key}>{subject}</Text>;
                        }
                      )}
                  </Box>
                  <Box>
                    <Heading size={"sm"} mb={2}>
                      Consumers
                    </Heading>
                    {streamData.consumers != undefined &&
                    streamData.consumers.length != 0 ? (
                      streamData.consumers.map(
                        (consumer: string, key: number) => {
                          return <Text key={key}>{consumer}</Text>;
                        }
                      )
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
              )
            )}
          </VStack>
        </Card>
      )}
    </Stack>
  );
}

export { StreamView };
