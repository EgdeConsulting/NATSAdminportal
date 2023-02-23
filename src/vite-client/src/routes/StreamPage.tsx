import { Card, CardBody, VStack } from "@chakra-ui/react";
import { useEffect, useState } from "react";
import { StreamView } from "components";

function StreamPage() {
  const [streams, setStreams] = useState<[]>([]);

  useEffect(() => {
    getStreams();
  }, [streams.length != 0]);

  function getStreams() {
    fetch("/api/streamBasicInfo")
      .then((res) => res.json())
      .then((data) => {
        setStreams(data);
      });
  }

  return (
    <VStack align="stretch" paddingTop={2}>
      <Card variant={"outline"} width={"1115px"} padding={2}>
        <CardBody>
          <StreamView streamInfo={streams} />
        </CardBody>
      </Card>
    </VStack>
  );
}

export { StreamPage };
