import { Flex, Card, CardBody, HStack, StackDivider } from "@chakra-ui/react";
import { MsgIcon, StreamIcon } from "assets";
import { PageHeader } from "components";
import { QuickStartCard } from "components/utils/QuickStartCard";

function HomePage() {
  return (
    <Flex w={"100%"} role={"main"}>
      <Card variant={"outline"} w={"100%"} mt={2} mr={2}>
        <CardBody>
          <PageHeader
            centerContent={true}
            heading={"Welcome to Egdes NATS-administration portal"}
            introduction={"A quick overview presented below"}
          />
          <HStack w={"100%"} mt={10}>
            <QuickStartCard
              header="Messages"
              image={<MsgIcon />}
              route="/messages"
              description="Includes a message table for all messages that reside in the server and optionally filter them out based on certain properties."
            />
            <QuickStartCard
              header="Streams"
              image={<StreamIcon />}
              route="/streams"
              description="Includes a stream table for all the streams that reside on the server and optionally filter them out based on certain properties."
            />
          </HStack>
        </CardBody>
      </Card>
    </Flex>
  );
}

export { HomePage };
