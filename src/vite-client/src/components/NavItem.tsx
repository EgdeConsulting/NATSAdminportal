import { Flex, Menu, MenuButton, Link, Icon, Text } from "@chakra-ui/react";
import { IconType } from "react-icons";

function NavItem({
  navSize,
  icon,
  title,
}: {
  navSize: number;
  icon: IconType;
  title: string;
}) {
  return (
    <Flex
      mt={30}
      flexDir="column"
      w="100%"
      alignItems={navSize == 200 ? "center" : "flex-start"}
    >
      <Menu placement="right">
        <Link
          _hover={{ textDecor: "none", backgroundColor: "gray.500" }}
          p={2}
          borderRadius={8}
        >
          <MenuButton w="100%">
            <Flex>
              <Icon as={icon} fontSize="xl" />
              <Text
                ml={5}
                align="center"
                display={navSize == 200 ? "none" : "flex-start"}
              >
                {title}
              </Text>
            </Flex>
          </MenuButton>
        </Link>
      </Menu>
    </Flex>
  );
}
export { NavItem };
