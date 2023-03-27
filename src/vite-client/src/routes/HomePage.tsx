import { Flex, Card, CardBody, HStack } from "@chakra-ui/react";
import { PageHeader } from "components";
import { QuickStartCard } from "components/utils/QuickStartCard";
import { FiMail, FiLayers } from "react-icons/fi";

function HomePage() {
  return (
    <Flex w={"100%"}>
      <Card variant={"outline"} w={"100%"} mt={2} mr={2}>
        <CardBody>
          <PageHeader
            heading={"Welcome to Egdes NATS-administration portal"}
            introduction={"A quick overview presented below"}
          />
          <HStack w={"100%"} mt={10}>
            <QuickStartCard
              header="Messages"
              image={FiMail}
              route="/messages"
              description="Includes a message table for  all messages that reside in the server and optionally filter them out based on certain properties"
            />
            <QuickStartCard
              header="Streams"
              image={FiLayers}
              route="/streams"
              description="Includes a stream table for all the streams that reside on the server and optionally filter them out based on certain properties"
            />
          </HStack>
        </CardBody>
      </Card>
    </Flex>
  );
}

export { HomePage };
